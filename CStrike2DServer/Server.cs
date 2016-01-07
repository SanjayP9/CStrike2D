using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
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

        private static List<Player> players;
        private static short playerIdentifier = 0;
        private static string serverVersion = "0.1.0a";            // Server Version
        private static int maxPlayers = 32;
        private static int port = 27015;
        private static string buffer = "";
        private static string serverName = "Global Offensive Server - " + serverVersion;
        private static bool forceConfigRewrite = true;
        static int curRow = 3;

        static void Main(string[] args)
        {

            Vector2 defSpawnPosition = new Vector2(350, 350);

            Stopwatch sw = new Stopwatch();

            players = new List<Player>();

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

                if (sw.Elapsed.Milliseconds >= 2)
                {
                    tick++;
                    Update();
                    sw.Restart();

                    if (tick == 50)
                    {
                        Console.SetCursorPosition(0, 0);
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
                NetOutgoingMessage outMsg = server.CreateMessage();
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
                                msg.SenderConnection.SendMessage(outMsg, NetDeliveryMethod.UnreliableSequenced, 0);
                                break;
                            case NetConnectionStatus.Disconnected:
                                player = players.Find(ply => ply.Client.RemoteUniqueIdentifier == msg.SenderConnection.RemoteUniqueIdentifier);

                                outMsg.Write(NetInterface.PLAYER_DC);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
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
                                players.Add(player);

                                Console.WriteLine("Player: \"" + player.PlayerName + "\" Connected. Identifier: " + playerIdentifier);
                               
                                // Send data about the new player to all connected players
                                foreach (Player plyr in players)
                                {
                                    // If the data we are sending is not the player themself
                                    outMsg = server.CreateMessage();
                                    outMsg.Write(NetInterface.SYNC_NEW_PLAYER);
                                    outMsg.Write(plyr.PlayerName);
                                    outMsg.Write(plyr.PlayerID);
                                    outMsg.Write((long)plyr.GetPosition().X);
                                    outMsg.Write((long)plyr.GetPosition().Y);
                                    outMsg.Write((long)plyr.Rotation);
                                    server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                                    Console.WriteLine("Sent data about \"" + player.PlayerName + "\"" +
                                                      " to player \"" + plyr.PlayerName + "\"");
                                }
                                Console.WriteLine("Sync Complete.");
                                break;
                            case NetInterface.MOVE_UP:
                                player.Move(0);
                                outMsg.Write(NetInterface.MOVE_UP);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.MOVE_DOWN:
                                player.Move(1);
                                outMsg.Write(NetInterface.MOVE_DOWN);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.MOVE_LEFT:
                                player.Move(2);
                                outMsg.Write(NetInterface.MOVE_LEFT);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.MOVE_RIGHT:
                                player.Move(3);
                                outMsg.Write(NetInterface.MOVE_RIGHT);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                break;
                            case NetInterface.FIRE:
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
    }
}