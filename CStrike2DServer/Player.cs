using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    class Player
    {

        private Vector2 position;
        private NetConnection senderConnection;
        private int count;

        public float Rotation { get; private set; }

        public string PlayerName { get; private set; }

        public long Client { get; private set; }

        public int PlayerID { get; private set; }

        public Player(string playerName, long client, int playerID)
        {
            PlayerName = playerName;
            position = new Vector2(0, 0);
            Client = client;
            PlayerID = playerID;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void Move(int direction)
        {
            switch (direction)
            {
                case 0: // UP
                    position.Y -= 0.5f;
                    break;
                case 1: // DOWN
                    position.Y += 0.5f;
                    break;
                case 2: // LEFT
                    position.X -= 0.5f;
                    break;
                case 3: // RIGHT
                    position.X += 0.5f;
                    break;
            }
        }

    }
}
