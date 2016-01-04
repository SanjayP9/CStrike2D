using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace CStrike2DServer
{
    class Server
    {

        static void Main(string[] args)
        {
            string serverVersion = "0.0.1a";            // Server Version
            int port = 27015;
            string serverName = "Global Offensive Server - " + serverVersion;

            Console.WriteLine("==================================");
            Console.WriteLine("Global Offensive - Version " + serverVersion);
            Console.WriteLine("==================================");

            Console.WriteLine("Loading config file...");
            ReadConfig();

            Console.WriteLine("Booting up server...");

            NetPeerConfiguration config = new NetPeerConfiguration("cstrike");
            config.Port = port;
            NetServer server = new NetServer(config);
            server.Start();

            Console.WriteLine("Server is live.");

            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Error:
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        break;
                    case NetIncomingMessageType.Data:
                        Console.WriteLine(msg.ReadString());
                        break;
                    case NetIncomingMessageType.Receipt:
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                }
            }

            Console.ReadLine();
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