using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    class ClientWeapon
    {

        public ClientPlayer Owner { get; private set; }
        public ParticleEmitterModel GunEmitter { get; private set; }
        public bool Fired { get; private set; }

        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public void FireWeapon()
        {
            if (!Fired)
            {
                Fired = true;
            }
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


        public void Draw(SpriteBatch sb)
        {
            // Draw particles
        }
    }
}
