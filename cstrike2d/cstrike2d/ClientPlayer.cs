using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    internal class ClientPlayer : Entity
    {
        public string UserName { get; private set; }
        public long UniqueIdentifier { get; private set; }
        public short EntityIdentifier { get; private set; }

        public PlayerState State { get; private set; }
        public Team CurrentTeam { get; private set; }
        public int Health { get; private set; }
        public int Armor { get; private set; }

        // Weapons
        public ClientWeapon CurrentWeapon { get; private set; }
        public ClientWeapon PrimaryWeapon { get; private set; }
        public ClientWeapon SecondaryWeapon { get; private set; }

        private RayEmitter rayEmitter;

        // State

        // Gets the position of the player
        public override Vector2 Position
        {
            get
            {
                return position;
            }
            protected set
            {
                position = value;
            }
        }

        public override int DrawOrder { get; protected set; }

        public override Rectangle Dimensions { get; protected set; }

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

        public ClientPlayer(string username, long identifier, Assets assets) : base(assets)
        {
            UserName = username;
            UniqueIdentifier = identifier;
            CurrentTeam = Team.Spectator;
            State = PlayerState.Dead;
            Health = 100;
            Armor = 0;
        }

        public override void Update(float gameTime)
        {

        }

        public override void Draw(SpriteBatch sb)
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

        public void Damage(int health, int armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}