// Author: Mark Voong
// File Name: DemoRecorder.cs
// Project Name: Global Offensive
// Creation Date: Jan 18th, 2016
// Modified Date: Jan 18th, 2016
// Description: Records a snapshot of the game, every few ticks
//              These snapshots are saved to a file, which is compressed
//              for distribution and is uncompressed to view the playback
using System;
using System.Collections.Generic;
using System.IO;

namespace CStrike2DServer
{
    class DemoRecorder
    {
        private int tick;                   // Current tick
        private int flushCount;             // Ticks before flushing to disk
        private int numTicks;               // Number of ticks that have passed in total
        private StreamWriter writer;        // Used to write to disk
        private string fileName;            // File name to write to disk
        private Random rand = new Random(); // Random generator

        /// <summary>
        /// The current state of the recorder
        /// </summary>
        public State RecorderState { get; private set; }

        public enum State
        {
            InActive,
            Recording,
        }

        /// <summary>
        /// Initializes a recorder that saves the state of a game
        /// </summary>
        public DemoRecorder()
        {
            RecorderState = State.InActive;
            tick = 0;
            flushCount = 0;
        }


        /// <summary>
        /// Starts recording
        /// </summary>
        /// <param name="mapName"></param>
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

        /// <summary>
        /// Ends the recording, saves, then closes the file
        /// </summary>
        public void EndRecording()
        {
            writer.WriteLine(numTicks);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Update logic
        /// </summary>
        public void Update()
        {
            tick++;

            // Every 8 ticks, take a snapshot of the world
            if (tick == 8)
            {
                SnapShot();
                tick = 0;
                flushCount++;
                numTicks++;
            }

            // Every 320 ticks, flush the data to disk to ensure
            // not as much data is lost if the server crashes
            if (flushCount == 40)
            {
                writer.Flush();
                flushCount = 0;
            }
        }

        /// <summary>
        /// Takes a snapshot of the current players in the game
        /// </summary>
        public void SnapShot()
        {
            List<ServerPlayer> players = Server.RetrievePlayers();

            // Take the number of players and each player's position and rotation
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
