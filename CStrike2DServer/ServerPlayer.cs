// Author: Mark Voong
// File Name: ServerPlayer.cs
// Project Name: Global Offensive
// Creation Date: Jan 15th, 2016
// Modified Date: Jan 19th, 2016
// Description: Serverside version of the player class. Handles all
//              serverside logic of a player
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    public class ServerPlayer
    {
        public string UserName { get; private set; }
        public short Identifier { get; private set; }
        public long ConnectionIdentifier { get; private set; }
        public short EntityIdentifier { get; private set; }

        public ServerClientInterface.PlayerState State { get; private set; }
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
            State = ServerClientInterface.PlayerState.Dead;
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
            float moveX = 0;
            float moveY = 0;
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    moveY -= 1f;
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    moveY += 1f;
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    moveX -= 1f;
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    moveX += 1f;
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    moveX -= 1f;
                    moveY -= 1f;
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    moveX += 1f;
                    moveY -= 1f;
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    moveX += 1f;
                    moveY += 1f;
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    moveX -= 1f;
                    moveY += 1f;
                    break;
            }

            Vector2 normalized = Vector2.Normalize(new Vector2(moveX, moveY));
            position += normalized * ServerClientInterface.MOVEMENT_SPEED;
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
                    SecondaryWeapon = new ServerWeapon(WeaponData.Weapon.Glock, this);
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

        public void SetState(ServerClientInterface.PlayerState state)
        {
            State = state;
        }

        public void SetWeapon(WeaponData.Weapon weapon)
        {
            PrimaryWeapon = new ServerWeapon(weapon, this);
            CurrentWeapon = PrimaryWeapon;
        }

        /// <summary>
        /// Kills the player
        /// </summary>
        public void Kill()
        {
            Health = 0;
            Armor = 0;
            State = ServerClientInterface.PlayerState.Dead;
            PrimaryWeapon.Drop();
        }

        /// <summary>
        /// Respawns the player if they had died
        /// </summary>
        public void Respawn(Vector2 location)
        {
            position = location;
            State = ServerClientInterface.PlayerState.Alive;
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