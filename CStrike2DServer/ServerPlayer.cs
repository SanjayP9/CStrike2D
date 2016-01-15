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
        }


        public void FireWeapon()
        {
            
        }

        public void SwitchWeapon()
        {
            
        }

        public void Move(byte direction)
        {
            
        }

        public void Damage(float health, float armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}