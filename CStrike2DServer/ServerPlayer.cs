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
        /// <summary>
        /// Username of the player
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Unique identifier for each player
        /// </summary>
        public short Identifier { get; private set; }

        /// <summary>
        /// Unique identiifer with the client that is associated to this player
        /// </summary>
        public long ConnectionIdentifier { get; private set; }

        /// <summary>
        /// Current state of the player
        /// </summary>
        public ServerClientInterface.PlayerState State { get; private set; }

        /// <summary>
        /// The palyer's current team
        /// </summary>
        public ServerClientInterface.Team CurrentTeam { get; private set; }

        /// <summary>
        /// Current health
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// Current armor
        /// </summary>
        public int Armor { get; private set; }

        /// <summary>
        /// Position of the player
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        private Vector2 position;   // The position of the player

        /// <summary>
        /// The rotation of the player
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// The player's current money
        /// </summary>
        public int Money { get; private set; }

        /// <summary>
        /// The weapon the player is currently holding
        /// </summary>
        public ServerWeapon CurrentWeapon { get; private set; }

        /// <summary>
        /// The player's priamry weapon
        /// </summary>
        public ServerWeapon PrimaryWeapon { get; private set; }

        /// <summary>
        /// The player's secondary weapon
        /// </summary>
        public ServerWeapon SecondaryWeapon { get; private set; }

        /// <summary>
        /// The player's knife
        /// </summary>
        public ServerWeapon Knife { get; private set; }

        /// <summary>
        /// Creates the server-side version of a player
        /// </summary>
        /// <param name="username"></param>
        /// <param name="identifier"></param>
        /// <param name="uniqueIdentifier"></param>
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

        /// <summary>
        /// Fires the player's current weapon
        /// </summary>
        public void FireWeapon()
        {
        }

        /// <summary>
        /// Switches the player's weapon
        /// </summary>
        public void SwitchWeapon()
        {
            
        }

        /// <summary>
        /// Moves the player in a direction
        /// </summary>
        /// <param name="direction"></param>
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

            // Normalize the vector so the player moves in all directions
            // in equal amounts
            Vector2 normalized = Vector2.Normalize(new Vector2(moveX, moveY));
            position += normalized * ServerClientInterface.MOVEMENT_SPEED;
        }

        /// <summary>
        /// Rotates thes the player
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(float rotation)
        {
            Rotation = rotation;
        }

        /// <summary>
        /// Changes the health of the player
        /// </summary>
        /// <param name="health"></param>
        public void SetHealth(int health)
        {
            Health = health;
        }
        
        /// <summary>
        /// Changes the armor of the player
        /// </summary>
        /// <param name="armor"></param>
        public void SetArmor(int armor)
        {
            Armor = armor;
        }

        /// <summary>
        /// Sets the team of the player
        /// </summary>
        /// <param name="team"></param>
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

        /// <summary>
        /// Sets the position of the player
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        /// <summary>
        /// Sets the state of the player
        /// </summary>
        /// <param name="state"></param>
        public void SetState(ServerClientInterface.PlayerState state)
        {
            State = state;
        }

        /// <summary>
        /// Sets the weapon of the player
        /// </summary>
        /// <param name="weapon"></param>
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
            Health = 100;
            Armor = 0;
            ResetWeapons();
        }

        /// <summary>
        /// Damages the player
        /// </summary>
        /// <param name="health"></param>
        /// <param name="armor"></param>
        public void Damage(int health, int armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}