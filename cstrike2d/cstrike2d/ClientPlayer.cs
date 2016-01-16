using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    internal class ClientPlayer
    {
        public string UserName { get; private set; }
        public long UniqueIdentifier { get; private set; }
        public short EntityIdentifier { get; private set; }

        public PlayerState State { get; private set; }
        public Team CurrentTeam { get; private set; }
        public float Health { get; private set; }
        public float Armor { get; private set; }

        // Weapons
        public ClientWeapon CurrentWeapon { get; private set; }
        public ClientWeapon PrimaryWeapon { get; private set; }
        public ClientWeapon SecondaryWeapon { get; private set; }

        // State

        // Gets the position of the player
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Gets the rotation of the player
        /// </summary>
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        private Vector2 position;
        private float rotation;

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

        public ClientPlayer(string username, long identifier)
        {
            UserName = username;
            UniqueIdentifier = identifier;
            CurrentTeam = Team.Spectator;
        }

        public void Update(float gameTime)
        {

        }

        public void Draw(SpriteBatch sb)
        {
            // Draw Player

            // Draw Weapon

        }


        public void Fire()
        {
            CurrentWeapon.FireWeapon();
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