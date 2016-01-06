using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CStrike2D;
using Lidgren.Network;

namespace CStrike2DServer
{
    class Server
    {
        private static NetServer server;
        private static NetPeerConfiguration config;
        private static NetIncomingMessage msg;

        static List<Player> players; 
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            string serverVersion = "0.0.1a";            // Server Version
            int port = 27015;
            string serverName = "Global Offensive Server - " + serverVersion;
            players = new List<Player>();

            Console.WriteLine("==================================");
            Console.WriteLine("Global Offensive - Version " + serverVersion);
            Console.WriteLine("==================================");

            Console.WriteLine("Loading config file...");
            ReadConfig();

            Console.WriteLine("Booting up server...");

            config = new NetPeerConfiguration("cstrike") {Port = port};
            server = new NetServer(config);
            server.Start();
            Console.WriteLine("Server is live.");

            sw.Start();
            int tick = 0;
            while (server.Status == NetPeerStatus.Running)
            {
                if (sw.Elapsed.Milliseconds >= 32)
                {
                    tick++;
                    Update();
                    sw.Restart();

                    if (tick == 4)
                    {
                        SyncServer();
                        tick = 0;
                    }
                }
            }
        }

        public static void SyncServer()
        {
            
        }

        public static void Update()
        {
            while ((msg = server.ReadMessage()) != null)
            {
                NetOutgoingMessage outMsg = server.CreateMessage();

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus) msg.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                outMsg.Write(NetInterface.HANDSHAKE);
                                msg.SenderConnection.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            case NetConnectionStatus.Disconnected:
                                Player plyr = players.Find(ply => ply.Client == msg.SenderConnection.RemoteUniqueIdentifier);
                                players.Remove(plyr);
                                Console.WriteLine("\"" + plyr.PlayerName + "\" has left the server");
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        byte identifier = msg.ReadByte();
                        Player player = players.Find(ply => ply.Client == msg.SenderConnection.RemoteUniqueIdentifier);
                        switch (identifier)
                        {
                            case NetInterface.HANDSHAKE:
                                string playerName = msg.ReadString();
                                Console.WriteLine("Player: \"" + playerName + "\" Connected.");
                                Player ply = new Player(playerName, msg.SenderConnection.RemoteUniqueIdentifier,
                                    players.Count);
                                players.Add(ply);
                                
                                // Send Data to all players
                                outMsg.Write(NetInterface.SYNC_NEW_PLAYER);
                                outMsg.Write(playerName);
                                outMsg.Write(ply.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                                Console.WriteLine("Sent player data to all connected clients");
                                break;
                            case NetInterface.MOVE_UP:
                                player.Move(0);
                                outMsg.Write(NetInterface.MOVE_UP);
                                outMsg.Write(player.PlayerID);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                Console.WriteLine(player);
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
                                Console.WriteLine(player.PlayerName + " fired weapon_ak47");
                                break;
                        }

                        /*
                        for (int i = 0; i < players.Count; i++)
                        {
                            outMsg.Write("PLYMOVE" + i);
                            outMsg.Write(players[i].GetPosition().X + "," + players[i].GetPosition().Y);
                            
                        }
                        */
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
                string[] commmands = File.ReadAllLines("config.txt");


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

            writer.Close();

            Console.WriteLine("Configuration written to disk.");
        }
    }
}