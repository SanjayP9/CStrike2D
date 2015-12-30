using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2DServer
{
    class Program
    {

        static void Main(string[] args)
        {
            // Server Version
            string serverVersion = "0.0.1a";

            Console.WriteLine("==================================");
            Console.WriteLine("Global Offensive - Version " + serverVersion);
            Console.WriteLine("==================================");

            Console.WriteLine("Loading config file...");


            Console.ReadLine();
        }

        static void ReadConfig()
        {
            // If the config file exists, open it and process it
            if (File.Exists("config.txt"))
            {
                string[] commmands = File.ReadAllLines("config.txt");
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
            StreamWriter writer = File.CreateText("config.txt");

            // Default settings

            writer.Close();
        }
    }
}
