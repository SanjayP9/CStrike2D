using CStrike2D;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    class Player
    {
        // SERVER-SIDE PLAYER ENTITY

        private Vector2 position;
        private NetConnection senderConnection;
        private int count;

        public float Rotation { get; private set; }

        public string PlayerName { get; private set; }

        public NetConnection Client { get; private set; }

        public short PlayerID { get; private set; }

        public short CurrentWeapon { get; private set; }

        public NetInterface.Team Team { get; private set; }

        public Player(string playerName, NetConnection client, short playerID)
        {
            PlayerName = playerName;
            position = new Vector2(0, 0);
            Client = client;
            PlayerID = playerID;
            Team = NetInterface.Team.Spectator;
        }

        public void ChangeTeam(NetInterface.Team newTeam)
        {
            Team = newTeam;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void Move(byte direction)
        {
            switch (direction)
            {
                case NetInterface.MOVE_UP: // UP
                    position.Y -= 5f;
                    break;
                case NetInterface.MOVE_DOWN: // DOWN
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_LEFT: // LEFT
                    position.X -= 5f;
                    break;
                case NetInterface.MOVE_RIGHT: // RIGHT
                    position.X += 5f;
                    break;
                case NetInterface.MOVE_UPRIGHT:
                    position.X += 5f;
                    position.Y -= 5f;
                    break;
                case NetInterface.MOVE_DOWNRIGHT:
                    position.X += 5f;
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_DOWNLEFT:
                    position.X -= 5f;
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_UPLEFT:
                    position.X -= 5f;
                    position.Y -= 5f;
                    break;
            }
        }
    }
}