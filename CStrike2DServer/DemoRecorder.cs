using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2DServer
{
    class DemoRecorder
    {
        private int tick;
        private int flushCount;
        private int numTicks;
        private StreamWriter writer;
        private string fileName;
        private Random rand = new Random();

        public State RecorderState { get; private set; }

        public enum State
        {
            InActive,
            Recording,
        }

        public DemoRecorder()
        {
            RecorderState = State.InActive;
            tick = 0;
            flushCount = 0;
        }


        public void StartRecording(string mapName)
        {
            RecorderState = State.Recording;

            fileName = mapName + "_demo_" + rand.Next(0, 1000) + ".txt";
            while (File.Exists(fileName))
            {
                fileName = mapName + "_demo_" + rand.Next(0, 1000) + ".txt";
            }

            writer = File.CreateText(fileName);
        }

        public void EndRecording()
        {
            writer.WriteLine(numTicks);
            writer.Flush();
            writer.Close();
        }

        public void Update()
        {
            tick++;

            if (tick == 8)
            {
                SnapShot();
                tick = 0;
                flushCount++;
                numTicks++;
            }

            if (flushCount == 40)
            {
                writer.Flush();
                flushCount = 0;
            }
        }

        public void SnapShot()
        {
            List<ServerPlayer> players = Server.RetrievePlayers();

            writer.WriteLine(players.Count);
            foreach (ServerPlayer player in players)
            {
                writer.WriteLine(player.Identifier);
                writer.WriteLine(player.Position.X);
                writer.WriteLine(player.Position.Y);
                writer.WriteLine(player.Rotation);
            }
        }
    }
}
