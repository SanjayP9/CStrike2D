// Author: Mark Voong
// File Name: ClientPlayer.cs
// Project Name: Global Offensive
// Creation Date: Jan 15th, 2016
// Modified Date: Jan 20th, 2016
// Description: Handles logic for the player.

using System;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class ClientPlayer : Entity
    {
        /// <summary>
        /// Player's name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Player's unique identifier
        /// </summary>
        public short Identifier { get; private set; }

        /// <summary>
        /// The player's current state
        /// </summary>
        public ServerClientInterface.PlayerState State { get; private set; }

        /// <summary>
        /// The player's current team
        /// </summary>
        public ServerClientInterface.Team CurrentTeam { get; private set; }

        /// <summary>
        /// The player's health
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// The player's armor
        /// </summary>
        public int Armor { get; private set; }

        // Weapons
        public ClientWeapon CurrentWeapon { get; private set; }
        public ClientWeapon PrimaryWeapon { get; private set; }
        public ClientWeapon SecondaryWeapon { get; private set; }
        public ClientWeapon Knife { get; private set; }


        /// <summary>
        /// Order at which the player is drawn in
        /// </summary>
        public override int DrawOrder { get; protected set; }

        // Dimensions of the player
        public override Rectangle Dimensions { get; protected set; }

        /// <summary>
        /// Raycast for drawing bullet lines
        /// </summary>
        public RayCast Shot { get; private set; }

        private ParticleEmitter debrisEmitter;

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

        private Vector2 position;   // Position of the player
        private float rotation;     // Rotation of the player

        /// <summary>
        /// Creates a client-side version of the player class
        /// </summary>
        /// <param name="username"></param>
        /// <param name="identifier"></param>
        /// <param name="assets"></param>
        public ClientPlayer(string username, short identifier, Assets assets)
            : base(assets)
        {
            UserName = username;
            Identifier = identifier;
            CurrentTeam = ServerClientInterface.Team.Spectator;
            State = ServerClientInterface.PlayerState.Dead;
            Health = 100;
            Armor = 0;
            PrimaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this, assets);
            SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this, assets);
            Knife = new ClientWeapon(WeaponData.Weapon.Knife, this, assets);
            CurrentWeapon = Knife;
            Shot = new RayCast();
            debrisEmitter = new ParticleEmitter(Particle.ParticleTypes.Debris);
        }

        /// <summary>
        /// Update logic for player
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(float gameTime)
        {
            CurrentWeapon.Update(gameTime);
            //PrimaryWeapon.Update(gameTime);
            //SecondaryWeapon.Update(gameTime);
        }

        /// <summary>
        /// Draws player and weapons
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            // If the player is not a spectator and isn't dead
            if (CurrentTeam != ServerClientInterface.Team.Spectator && State != ServerClientInterface.PlayerState.Dead)
            {
                if (CurrentTeam == ServerClientInterface.Team.CounterTerrorist)
                {
                    sb.Draw(Assets.CTTexture, position, new Rectangle(0, 0, 32, 32), Color.White, 1.57f + rotation,
                        new Vector2(16, 16),
                        1f, SpriteEffects.None, 0);
                }
                else
                {
                    sb.Draw(Assets.TTexture, position, new Rectangle(0, 0, 32, 32), Color.White, 1.57f + rotation,
                        new Vector2(16, 16),
                        1f, SpriteEffects.None, 0);
                }

                // Draw their name
                sb.DrawString(Assets.DefaultFont, UserName, new Vector2(position.X, position.Y - 50),
                    CurrentTeam == ServerClientInterface.Team.CounterTerrorist ? Color.Blue : Color.Red);

                // Draws the weapon
                CurrentWeapon.Draw(sb);

                // Draws the current weapon
                if (CurrentWeapon.Fired)
                {
                    Shot.Draw(sb, Assets.PixelTexture);
                }

                debrisEmitter.Draw(sb, Assets.DebrisParticle);
            }
        }

        /// <summary>
        /// Fires the current weapon
        /// </summary>
        public void Fire()
        {
            // If the weapon has not been fired yet
            if (!CurrentWeapon.Fired)
            {
                CurrentWeapon.FireWeapon();
                Shot.Update(position, 1280, Assets.MapData.TileMap, Assets.MapData.MapArea, rotation);
                Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                //debrisEmitter.Launch(Shot.CollisionPos, rotation);
            }
        }

       
        // UNFINISHED
        public void SwitchWeapon()
        {

        }

        public void Plant()
        {
            
        }

        public void Defuse()
        {
            
        }
         

        /// <summary>
        /// Used by the server
        /// </summary>
        /// <param name="weapon"></param>
        public void SetCurrentWeapon(WeaponData.Weapon weapon)
        {
            CurrentWeapon = new ClientWeapon(weapon, this, Assets);
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
                    SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.Usp, this, Assets);
                    break;
                case ServerClientInterface.Team.Terrorist:
                    SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.Glock, this, Assets);
                    break;
            }
            PrimaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this, Assets);
            Knife = new ClientWeapon(WeaponData.Weapon.Knife, this, Assets);
            CurrentWeapon = Knife;
        }


        /// <summary>
        /// Moves the player
        /// </summary>
        /// <param name="direction"></param>
        public void Move(byte direction)
        {
            float moveX = 0;
            float moveY = 0;
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    moveY -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    moveY += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    moveX -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    moveX += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    moveX -= ServerClientInterface.MOVEMENT_SPEED;
                    moveY -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    moveX += ServerClientInterface.MOVEMENT_SPEED;
                    moveY -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    moveX += ServerClientInterface.MOVEMENT_SPEED;
                    moveY += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    moveX -= ServerClientInterface.MOVEMENT_SPEED;
                    moveY += ServerClientInterface.MOVEMENT_SPEED;
                    break;
            }

            Vector2 normalized = Vector2.Normalize(new Vector2(moveX, moveY));
            position += normalized * ServerClientInterface.MOVEMENT_SPEED;
        }

        /// <summary>
        /// Sets the player's health
        /// </summary>
        /// <param name="health"></param>
        public void SetHealth(int health)
        {
            Health = health;
        }

        /// <summary>
        /// Sets the player's armor
        /// </summary>
        /// <param name="armor"></param>
        public void SetArmor(int armor)
        {
            Armor = armor;
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
        /// Sets the rotation of the player
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(float rotation)
        {
            this.rotation = rotation;
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
        /// Sets the state of the player given the byte
        /// </summary>
        /// <param name="state"></param>
        public void SetState(byte state)
        {
            State = ServerClientInterface.ByteToState(state);
        }

        /// <summary>
        /// Sets the state of the player given the enum
        /// </summary>
        /// <param name="state"></param>
        public void SetState(ServerClientInterface.PlayerState state)
        {
            State = state;
        }

        /// <summary>
        /// Respawns the player
        /// </summary>
        /// <param name="location"></param>
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
        /// Damaages the player
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