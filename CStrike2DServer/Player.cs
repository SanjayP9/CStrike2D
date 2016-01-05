﻿using System;
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

        public NetConnection Client { get; private set; }

        public byte PlayerID { get; private set; }

        public Player(string playerName, NetConnection client, byte playerID)
        {
            position = new Vector2(0, 0);
            Client = client;
            PlayerID = playerID;
        }

        public Player(string playerName, NetConnection senderConnection, int count)
        {
            PlayerName = playerName;
            this.senderConnection = senderConnection;
            this.count = count;
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