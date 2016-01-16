using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2DServer
{
    class ServerPlayer
    {
        public string UserName { get; private set; }
        public long UniqueIdentifier { get; private set; }
        public short EntityIdentifier { get; private set; }

        public PlayerState State { get; private set; }
        public Team CurrentTeam { get; private set; }
        public float Health { get; private set; }
        public float Armor { get; private set; }

        public enum PlayerState
        {
            Dead,
            Alive
        }

        public enum Team
        {
            CounterTerrorist,
            Terrorist,
            Spectator
        }

        public ServerPlayer(string username, long identifier)
        {
            UserName = username;
            UniqueIdentifier = identifier;
            CurrentTeam = Team.Spectator;
            State = PlayerState.Dead;
        }


        public void FireWeapon()
        {
            
        }

        public void SwitchWeapon()
        {
            
        }

        public void Move(byte direction)
        {
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    break;
            }
        }

        public void Damage(float health, float armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}