//
//
//
//
//
//
//

using System;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class ClientWeapon
    {

        public ClientPlayer Owner { get; private set; }
        public ParticleEmitter GunEmitter { get; private set; }
        public bool Fired { get; private set; }

        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        // Enum that specifies the clients currentWepon
        public WeaponData.Weapon Weapon { get; private set; }

        // Stores an instance of assests in order to use textures
        private Assets assets;

        public float FireRate { get; private set; }

        // Used to store an instance of a ParticleEmitter
        private ParticleEmitter gunSmokeParticle;
        private ParticleEmitter shellParticle;

        public bool FireWeapon()
        {
            if (!Fired)
            {
                                
                Texture2D gunTexture = assets.GetWeaponTexture(Weapon);

              
                Vector2 direction = new Vector2(
                    (float)Math.Cos(Rotation) * gunTexture.Width, 
                    (float)Math.Sin(Rotation) * gunTexture.Width);
                //Vector2 barrelVect = new Vector2(Position.X + (gunTexture.Width ), Position.Y + (gunTexture.Height * 0.5f));

                gunSmokeParticle.Launch(direction + Position, Rotation);

                shellParticle.Launch(new Vector2(Position.X + direction.X/2f, Position.Y + direction.Y), Rotation - 1.57f);

                Fired = true;
                FireRate = WeaponData.FireRate(Weapon);
            }
            return Fired;
        }

        public ClientWeapon(WeaponData.Weapon weapon, ClientPlayer owner, Assets assets)
        {
            Owner = owner;
            Weapon = weapon;
            this.assets = assets;

            gunSmokeParticle = new ParticleEmitter(Particle.ParticleTypes.GunSmoke);
            shellParticle = new ParticleEmitter(Particle.ParticleTypes.Shell);
        }

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

        public void Pickup(ClientPlayer owner)
        {
            Owner = owner;
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw particles
            sb.Draw(assets.GetWeaponTexture(Weapon), Position, null, Color.White, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            gunSmokeParticle.Draw(sb, assets.ParticleTexture);
            shellParticle.Draw(sb, assets.ShellTexture);
        }
    }
}
