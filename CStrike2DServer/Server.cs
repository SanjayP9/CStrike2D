using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CStrike2D;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    class Server
    {
        private static NetServer server;
        private static NetPeerConfiguration config;
        private static NetIncomingMessage msg;

        private static List<Player> players = new List<Player>();
        private static short playerIdentifier;
        private static string serverVersion = "0.2.3a";            // Server Version
        private static int maxPlayers = 32;
        private static int port = 27015;
        private static string buffer = "";
        private static string serverName = "Global Offensive Server - " + serverVersion;
        private static bool forceConfigRewrite = true;
        private static NetOutgoingMessage outMsg;
        static int curRow = 3;
        private static int tickCount;
        private static int bytesIn;
        private static int bytesOut;
        private static int maxCTs = 16;
        private static int maxTs = 16;
        private static bool enableCollision = true;
        private static short entityCounter;

        static void Main(string[] args)
        {
            /* Ballpark estimates for bandwidth usage
             * - Each command requires 1 byte for the identifier.
             * - Each player sends input requests of the following:
             * - Direction (2 Bytes)
             * - Rotation (3 Bytes)
             * - Player Updates at 60Hz
             * - 5 bytes * 60 times a second = 0.3kb/s Upload
             * - The server must send this to every player on the server on demand
             * - Direction (3 Bytes)
             * - Rotation (5 Bytes)
             * - Server Updates at 64Hz up to 128Hz
             * - At 64Hz
             * - 8 bytes * 64 times a second * 32 players = 16kb/s Upload
             * - At 128Hz
             * - 8 bytes * 128 times a second * 32 players = 32kb/s Upload
             * - The server syncs everyone's postion and rotation every 60 ticks or ~930ms
             * - Direction (4 Bytes)
             * - Rotation (5 Bytes)
             * - PlayerID (2 Bytes)
             * - At 64 Hz
             * - 11 bytes * 64 times a second * 32 players = 22kb/s Upload
             * - At 128 Hz
             * - 11 bytes * 128 times a second * 32 players = 44kb/s Upload
             */

            Vector2 defSpawnPosition = new Vector2(350, 350);

            Stopwatch sw = new Stopwatch();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("==================================");
            Console.WriteLine("Global Offensive - Version " + serverVersion);
            Console.WriteLine("==================================");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Loading config file...");

            if (forceConfigRewrite)
            {
                WriteFile();
            }

            ReadConfig();

            Console.WriteLine("Booting up server...");

            config = new NetPeerConfiguration("cstrike") {Port = port, EnableUPnP = true};
            server = new NetServer(config);
            server.Start();
            Console.WriteLine("Server is live.");

            sw.Start();
            int tick = 0;
            while (server.Status == NetPeerStatus.Running)
            {
                if (Console.KeyAvailable)
                {
                    buffer += Console.ReadKey();
                }

                if (sw.Elapsed.TotalMilliseconds >= 15.625)
                {
                    tick++;
                    Update();
                    bytesIn += server.Statistics.ReceivedBytes;
                    bytesOut += server.Statistics.SentBytes;

                    sw.Restart();

                    if (tick == 60)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("==================================");
                        Console.WriteLine("Global Offensive - Version " + serverVersion);
                        Console.WriteLine("==================================");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Players: " + players.Count + "/" + maxPlayers);

                        foreach (Player ply in players)
                        {
                            Console.WriteLine("Client: " + ply.PlayerName + " Ping: " + Math.Round(ply.Client.AverageRoundtripTime, 2) + "ms");
                        }
                        Console.WriteLine("KB In: " + bytesIn / 1024d + " KB Out: " + bytesOut / 1024d);
                        bytesIn = 0;
                        bytesOut = 0;
                        Console.WriteLine(buffer);

                        SyncServer();
                        tick = 0;
                    }
                }
            }
        }

        public static void SyncServer()
        {
            NetOutgoingMessage syncMsg;

            foreach (Player ply in players)
            {
                syncMsg = server.CreateMessage();
                syncMsg.Write(NetInterface.SYNC_MOVEMENT);
                syncMsg.Write(ply.PlayerID);
                syncMsg.Write(ply.GetPosition().X);
                syncMsg.Write(ply.GetPosition().Y);
                server.SendToAll(syncMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        public static void Update()
        {
            while ((msg = server.ReadMessage()) != null)
            {
                outMsg = server.CreateMessage();
                Player player;
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus) msg.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                outMsg.Write(NetInterface.HANDSHAKE);
                                playerIdentifier++;
                                outMsg.Write(playerIdentifier);
                                Console.WriteLine("Client given identifier: " + playerIdentifier);
                                msg.SenderConnection.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            case NetConnectionStatus.Disconnected:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);

                                outMsg.Write(NetInterface.PLAYER_DC);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                                Console.WriteLine("\"" + player.PlayerName + "\" has left the server");
                                players.Remove(player);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        byte identifier = msg.ReadByte();
                        player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                        switch (identifier)
                        {
                            case NetInterface.HANDSHAKE:
                                player = new Player(msg.ReadString(), msg.SenderConnection, playerIdentifier);
                                player.SetPosition(new Vector2(players.Count * 50, players.Count * 50));
                                player.SetCurrentWeapon(NetInterface.WEAPON_KNIFE, entityCounter);
                                players.Add(player);
                                entityCounter++;
                                Console.WriteLine("Player: \"" + player.PlayerName + "\" Connected. Identifier: " + playerIdentifier);
                               
                                // Send data about the new player to all connected players
                                foreach (Player plyr in players)
                                {
                                    outMsg = server.CreateMessage();
                                    outMsg.Write(NetInterface.SYNC_NEW_PLAYER);
                                    outMsg.Write(plyr.PlayerName);
                                    outMsg.Write(plyr.PlayerID);
                                    outMsg.Write(plyr.GetPosition().X);
                                    outMsg.Write(plyr.GetPosition().Y);
                                    outMsg.Write(plyr.Rotation);
                                    outMsg.Write(NetInterface.GetTeamByte(plyr.Team));
                                    outMsg.Write(entityCounter);
                                    outMsg.Write(plyr.CurrentWeapon);
                                    server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                                    Console.WriteLine("Sent data about \"" + player.PlayerName + "\"" +
                                                      " to player \"" + plyr.PlayerName + "\"");
                                }
                                Console.WriteLine("Sync Complete.");
                                break;
                            case NetInterface.MOVE_UP:
                            case NetInterface.MOVE_DOWN:
                            case NetInterface.MOVE_LEFT:
                            case NetInterface.MOVE_RIGHT:
                            case NetInterface.MOVE_UPRIGHT:
                            case NetInterface.MOVE_DOWNRIGHT:
                            case NetInterface.MOVE_DOWNLEFT:
                            case NetInterface.MOVE_UPLEFT:
                                if (CheckPlayerCollision(player, identifier))
                                {
                                    Move(identifier, player);
                                }
                                break;
                            case NetInterface.FIRE:
                                foreach (Player ply in players)
                                {
                                    if (player.PlayerID != ply.PlayerID)
                                    {
                                        if (Collision.BulletToPerson(player.GetPosition(), ply.GetPosition(),
                                                player.Rotation, 23f))
                                        {
                                            Console.WriteLine("Collision");
                                        }
                                    }
                                }
                                outMsg.Write(NetInterface.PLAY_SOUND);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(NetInterface.AK47_SHOT);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.ROTATE:
                                outMsg.Write(NetInterface.ROTATE);
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                player.SetRotation(msg.ReadFloat());
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(player.Rotation);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.PLY_CHANGE_TEAM:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                player.ChangeTeam(NetInterface.GetTeam(msg.ReadByte()));
                                outMsg.Write(NetInterface.PLY_CHANGE_TEAM);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(NetInterface.GetTeamByte(player.Team));
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                                break;
                            case NetInterface.SPAWN_WEAPON:
                                short weapon = msg.ReadInt16();
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                if (WeaponInfo.GetWeaponType(WeaponInfo.GetWeapon(weapon)) ==
                                    WeaponInfo.WeaponType.Primary)
                                {
                                    player.SetPrimaryWeapon(weapon, entityCounter);
                                    player.SetCurrentWeapon(weapon, player.PrimaryWepEntID);

                                }
                                else
                                {
                                    player.SetSecondaryWeapon(weapon, entityCounter);
                                    player.SetCurrentWeapon(weapon, player.PrimaryWepEntID);
                                }
                                outMsg.Write(NetInterface.SPAWN_WEAPON);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(entityCounter);
                                outMsg.Write(weapon);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                                entityCounter++;
                                break;
                            case NetInterface.SWITCH_WEAPON:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);
                                player.SetCurrentWeapon(msg.ReadInt16(), msg.ReadInt16());
                                outMsg.Write(NetInterface.SWITCH_WEAPON);
                                outMsg.Write(player.PlayerID);
                                outMsg.Write(player.CurrentWeapon);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                        }
                        break;
                }
            }
            server.Recycle(msg);
        }

        static void ReadConfig()
        {
            // If the config file exists, open it and process it
            if (File.Exists("config.txt"))
            {
                string[] commands = File.ReadAllLines("config.txt");

                foreach (string command in commands)
                {
                    string[] cmd = command.Split('=');

                    switch (cmd[0])
                    {
                        case "servername=":
                            break;
                    }
                }

                Console.WriteLine("Configuration Successfully Loaded.");
            }
            else
            {
                WriteFile();
            }
        }

        /// <summary>
        /// Writes a fresh config to the directory if none exists
        /// </summary>
        static void WriteFile()
        {
            Console.WriteLine("Configuration not found, creating default configuration...");
            StreamWriter writer = File.CreateText("config.txt");

            // Default settings
            writer.WriteLine("cstrikeserverconfig=" + serverVersion);
            writer.WriteLine("servername=" + serverName);
            writer.WriteLine("port=" + port);
            writer.WriteLine("maxplayers=" + maxPlayers);

            writer.Close();

            Console.WriteLine("Configuration written to disk.");
        }

        static void Move(byte direction, Player player)
        {
            if (CheckPlayerCollision(player, direction))
            {
                player.Move(direction);
                outMsg.Write(direction);
                outMsg.Write(player.PlayerID);
                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }

        static bool CheckPlayerCollision(Player player, byte direction)
        {
            if (player.Team != NetInterface.Team.Spectator && enableCollision)
            {
                float vectorX = 0f;
                float vectorY = 0f;
                switch (direction)
                {
                    case NetInterface.MOVE_UP:
                        vectorY = -5f;
                        break;
                    case NetInterface.MOVE_DOWN:
                        vectorY = 5f;
                        break;
                    case NetInterface.MOVE_LEFT:
                        vectorX = -5f;
                        break;
                    case NetInterface.MOVE_RIGHT:
                        vectorX = 5f;
                        break;
                    case NetInterface.MOVE_UPRIGHT:
                        vectorX = 5f;
                        vectorY = -5f;
                        break;
                    case NetInterface.MOVE_DOWNRIGHT:
                        vectorX = 5f;
                        vectorY = 5f;
                        break;
                    case NetInterface.MOVE_DOWNLEFT:
                        vectorX = -5f;
                        vectorY = 5f;
                        break;
                    case NetInterface.MOVE_UPLEFT:
                        vectorX = -5f;
                        vectorY = -5f;
                        break;
                }

                foreach (Player ply in players)
                {
                    if (ply.PlayerID != player.PlayerID)
                    {
                        if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + vectorX,
                            player.GetPosition().Y + vectorY), ply.GetPosition(), 23f))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return true;

            #region Old Code

            /*
            switch (direction)
            {
                case NetInterface.MOVE_UP:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X,
                                player.GetPosition().Y - 5f), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_DOWN:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X,
                                player.GetPosition().Y + 5f), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_LEFT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X - 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_RIGHT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_UPRIGHT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_DOWNRIGHT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_DOWNLEFT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X + 5f,
                                player.GetPosition().Y), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case NetInterface.MOVE_UPLEFT:
                    foreach (Player ply in players)
                    {
                        if (ply.PlayerID != player.PlayerID)
                        {
                            if (Collision.PlayerToPlayer(new Vector2(player.GetPosition().X - 5f,
                                player.GetPosition().Y - 5f), ply.GetPosition(), 23f))
                            {
                                return false;
                            }
                        }
                    }
                    break;
            }
            return true;
             */

            #endregion
        }
    }
}