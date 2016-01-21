// Author: Mark Voong
// File Name: ClientWeapon.cs
// Project Name: Global Offensive
// Creation Date: Jan 16th, 2016
// Modified Date: Jan 20th, 2016
// Description: Handles logic for weapons.
using System;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class ClientWeapon
    {
        /// <summary>
        /// The owner of the weapon
        /// </summary>
        public ClientPlayer Owner { get; private set; }

        /// <summary>
        /// The particle emitter for the weapon
        /// </summary>
        public ParticleEmitter GunEmitter { get; private set; }

        /// <summary>
        /// Was the weapon fired
        /// </summary>
        public bool Fired { get; private set; }

        /// <summary>
        /// Position of the weapon
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Rotation of the weapon
        /// </summary>
        public float Rotation { get; private set; }

        // Enum that specifies the clients currentWepon
        public WeaponData.Weapon Weapon { get; private set; }

        // Stores an instance of assests in order to use textures
        private Assets assets;

        /// <summary>
        /// The weapon's fire rate
        /// </summary>
        public float FireRate { get; private set; }

        // Used to store an instance of a ParticleEmitter
        private ParticleEmitter gunSmokeParticle;
        private ParticleEmitter shellParticle;

        /// <summary>
        /// Fires the weapon
        /// </summary>
        /// <returns></returns>
        public bool FireWeapon()
        {
            // If the weapon is currently not recovering from its fire rate
            if (!Fired)
            {
                // Get the texture of the gun 
                Texture2D gunTexture = assets.GetWeaponTexture(Weapon);

              
                // Set the emit direction to be the rotation of the weapon plus the
                // weapons width (in front of the gun)
                Vector2 direction = new Vector2(
                    (float)Math.Cos(Rotation) * gunTexture.Width, 
                    (float)Math.Sin(Rotation) * gunTexture.Width);

                // If the weapon is not a knife
                if (Weapon != WeaponData.Weapon.Knife)
                {
                    // Launch the particle
                    gunSmokeParticle.Launch(direction + Position, Rotation);

                    // Launch the shell
                    shellParticle.Launch(new Vector2(Position.X + direction.X/2f, Position.Y + direction.Y),
                        Rotation + 1.57f);
                }

                // Set weapon to fired until cooldown
                Fired = true;
                FireRate = WeaponData.FireRate(Weapon);
            }
            return Fired;
        }

        /// <summary>
        /// Creates a weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="owner"></param>
        /// <param name="assets"></param>
        public ClientWeapon(WeaponData.Weapon weapon, ClientPlayer owner, Assets assets)
        {
            Owner = owner;
            Weapon = weapon;
            this.assets = assets;

            gunSmokeParticle = new ParticleEmitter(Particle.ParticleTypes.GunSmoke);
            shellParticle = new ParticleEmitter(Particle.ParticleTypes.Shell);
        }

        /// <summary>
        /// Update logic for a weapon
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            // If the weapon is currently being "held" by a player,
            // if null, it is on the floor
            if (Owner != null)
            {
                Position = Owner.Position;
                Rotation = Owner.Rotation;

                if (Fired)
                {
                    if (FireRate >= 0f)
                    {
                        FireRate -= gameTime;
                    }
                    else
                    {
                        Fired = false;
                    }
                }
            }

            // Update particles
            gunSmokeParticle.Update(gameTime);
            shellParticle.Update(gameTime);
        }

        /// <summary>
        /// Drops the weapon 
        /// </summary>
        public void Drop()
        {
            Owner = null;
        }

        /// <summary>
        /// Allows a player to pickup the weapon
        /// </summary>
        /// <param name="owner"></param>
        public void Pickup(ClientPlayer owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Draws the weapon
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            // Draw particles and weapon
            sb.Draw(assets.GetWeaponTexture(Weapon), Position, null, Color.White, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            gunSmokeParticle.Draw(sb, assets.ParticleTexture);
            shellParticle.Draw(sb, assets.ShellTexture);
        }
    }
}
