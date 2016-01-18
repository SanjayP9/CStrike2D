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

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        private Vector2 position;
        public float Rotation { get; private set; }

        public int Money { get; private set; }

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
            PrimaryWeapon = new ServerWeapon(WeaponData.Weapon.None, this);
            SecondaryWeapon = new ServerWeapon(WeaponData.Weapon.None, this);
            CurrentWeapon = Knife;
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
                    position.Y -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    position.Y += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    position.X -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    position.X += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    position.X -= ServerClientInterface.MOVEMENT_SPEED;
                    position.Y -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    position.X += ServerClientInterface.MOVEMENT_SPEED;
                    position.Y -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    position.X += ServerClientInterface.MOVEMENT_SPEED;
                    position.Y += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    position.X -= ServerClientInterface.MOVEMENT_SPEED;
                    position.Y += ServerClientInterface.MOVEMENT_SPEED;
                    break;
            }
        }

        public void Rotate(float rotation)
        {
            Rotation = rotation;
        }

        public void SetHealth(int health)
        {
            Health = health;
        }

        public void SetArmor(int armor)
        {
            Armor = armor;
        }

        public void SetTeam(byte team)
        {
            CurrentTeam = ServerClientInterface.ByteToTeam(team);
        }

        /// <summary>
        /// If the player was dead last round, give them default weapons specific
        /// to their team
        /// </summary>
        public void ResetWeapons()
        {
            switch (CurrentTeam)
            {
                case ServerClientInterface.Team.CounterTerrorist:
                    SecondaryWeapon = new ServerWeapon(WeaponData.Weapon.Usp, this);
                    break;
                case ServerClientInterface.Team.Terrorist:
                    SecondaryWeapon = new ServerWeapon(WeaponData.Weapon.Glock18, this);
                    break;
            }
            CurrentWeapon = Knife;
        }

        /// <summary>
        /// Changes the person's cash by a certain amount. Used for
        /// buying weapons and kills
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeCash(int amount)
        {

        }

        /// <summary>
        /// Sets the person's cash. Used for when new players
        /// connect to the server
        /// </summary>
        /// <param name="amount"></param>
        public void SetCash(int amount)
        {
            
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetState(PlayerState state)
        {
            State = state;
        }

        /// <summary>
        /// Kills the player
        /// </summary>
        public void Kill()
        {
            Health = 0;
            Armor = 0;
            State = PlayerState.Dead;
            PrimaryWeapon.Drop();
        }

        /// <summary>
        /// Respawns the player if they had died
        /// </summary>
        public void Respawn(Vector2 location)
        {
            position = location;
            State = PlayerState.Alive;
            CurrentWeapon = Knife;
            ResetWeapons();
        }

        public void Damage(float health, float armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}