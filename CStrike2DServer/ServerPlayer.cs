using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    public class ServerPlayer
    {
        public string UserName { get; private set; }
        public short Identifier { get; private set; }
        public long ConnectionIdentifier { get; private set; }
        public short EntityIdentifier { get; private set; }

        public PlayerState State { get; private set; }
        public ServerClientInterface.Team CurrentTeam { get; private set; }
        public float Health { get; private set; }
        public float Armor { get; private set; }

        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }


        public ServerWeapon CurrentWeapon { get; private set; }
        public ServerWeapon PrimaryWeapon { get; private set; }
        public ServerWeapon SecondaryWeapon { get; private set; }
        public ServerWeapon Knife { get; private set; }

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

        public ServerPlayer(string username, short identifier, long uniqueIdentifier)
        {
            UserName = username;
            Identifier = identifier;
            ConnectionIdentifier = uniqueIdentifier;
            CurrentTeam = ServerClientInterface.Team.Spectator;
            State = PlayerState.Dead;
            Knife = new ServerWeapon(WeaponData.Weapon.Knife, this);
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

        public void SetHealth(int health)
        {
            Health = health;
        }

        public void SetArmor(int armor)
        {
            Armor = armor;
        }

        public void Damage(float health, float armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}