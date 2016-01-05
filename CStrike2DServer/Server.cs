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
            while (server.Status == NetPeerStatus.Running)
            {
                if (sw.Elapsed.Milliseconds >= 32)
                {
                    Update();
                    sw.Restart();
                }
            }
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
                                Console.WriteLine("Player has left the server");
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        byte identifier = msg.ReadByte();

                        switch (identifier)
                        {
                            case NetInterface.HANDSHAKE:
                                string playerName = msg.ReadString();
                                Console.WriteLine("Player: \"" + playerName + "\" Connected.");
                                players.Add(new Player(playerName, msg.SenderConnection, players.Count));
                                break;
                            case NetInterface.MOVE_UP:
                                players.Find(ply => ply.Client == msg.SenderConnection).Move(0);
                                break;
                            case NetInterface.MOVE_DOWN:
                                players.Find(ply => ply.Client == msg.SenderConnection).Move(1);
                                break;
                            case NetInterface.MOVE_LEFT:
                                players.Find(ply => ply.Client == msg.SenderConnection).Move(2);
                                break;
                            case NetInterface.MOVE_RIGHT:
                                players.Find(ply => ply.Client == msg.SenderConnection).Move(3);
                                break;
                            case NetInterface.FIRE:
                                outMsg.Write(NetInterface.PLAY_SOUND);
                                outMsg.Write(players.Find(ply => ply.Client == msg.SenderConnection).PlayerID);
                                outMsg.Write(NetInterface.AK47_SHOT);
                                server.SendToAll(outMsg, NetDeliveryMethod.UnreliableSequenced);
                                Console.WriteLine("AK47 Shot");
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