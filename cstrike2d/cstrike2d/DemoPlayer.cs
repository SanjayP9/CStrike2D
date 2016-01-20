// Author: Mark Voong
// File Name: DemoPlayer.cs
// Project Name: Global Offensive
// Creation Date: Jan 18th, 2015
// Modified Date: Jan 19th, 2016
// Description: Handles playback of demo files which are
//              recordings of gameplay by the server
using System;
using System.Collections.Generic;
using System.IO;

namespace CStrike2D
{
    class DemoPlayer
    {
        private SnapShot[] snapshots;

        public DemoPlayer()
        {
            
        }


        public void Update()
        {
        }

        public void LoadFile(string fileName)
        {
            string[] data = File.ReadAllLines(fileName);
            snapshots = new SnapShot[Convert.ToInt32(data[data.Length -1])];

            for (int i = 0; i < data.Length - 2; i++)
            {
                int numPlayers = Convert.ToInt32(data[i]);
                for (int j = i; j < numPlayers; j++)
                {
                    
                }
            }
        }
    }

    public class SnapShot
    {
        private List<ClientPlayer> players = new List<ClientPlayer>();

        public void AddPlayer(ClientPlayer player)
        {
            players.Add(player);
        }
    }
}
