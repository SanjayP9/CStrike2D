using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
