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

        public WeaponData.Weapon Weapon { get; private set; }

        private Assets assets;

        public void FireWeapon()
        {
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
        }
    }
}
