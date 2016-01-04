using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace CStrike2DServer
{
    class Server
    {
        private static NetServer server;
        private static NetPeerConfiguration config;
        private static NetIncomingMessage msg;
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            string serverVersion = "0.0.1a";            // Server Version
            int port = 27015;
            string serverName = "Global Offensive Server - " + serverVersion;

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
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus) msg.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                NetOutgoingMessage outMsg = server.CreateMessage();
                                outMsg.Write("welcome");
                                msg.SenderConnection.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine("Player has left the server");
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        string line = msg.ReadString();
                        if (line.Contains("HNDSHAKE"))
                        {
                            Console.WriteLine("Player: \"" + line.Substring(8, line.Length - 8) + "\" Connected.");
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