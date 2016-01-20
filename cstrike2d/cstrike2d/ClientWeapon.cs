//
//
//
//
//
//
//
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

        // Used to store an instance of a ParticleEmitter
        private ParticleEmitter gunSmokeParticle;

        public void FireWeapon()
        {
            Texture2D gunTexture = assets.GetWeaponTexture(Weapon);
            Vector2 barrelVect = new Vector2(Position.X + (gunTexture.Height), Position.Y + (gunTexture.Width * 0.5f));

            gunSmokeParticle.Launch(barrelVect, Owner.Rotation);

            if (!Fired)
            {
                Fired = true;
            }
        }

        public ClientWeapon(WeaponData.Weapon weapon, ClientPlayer owner, Assets assets)
        {
            Owner = owner;
            Weapon = weapon;
            this.assets = assets;

            gunSmokeParticle = new ParticleEmitter(Particle.ParticleTypes.GunSmoke);
        }

        public void Update(float gameTime)
        {
            // If the weapon is currently being "held" by a player,
            // if null, it is on the floor
            if (Owner != null)
            {
                Position = Owner.Position;
                Rotation = Owner.Rotation;
            }

            gunSmokeParticle.Update(gameTime);
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

            gunSmokeParticle.Draw(sb, assets.SmokeParticle);
        }
    }
}
