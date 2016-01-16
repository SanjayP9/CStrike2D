using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    class ClientGrenade
    {

        public Vector2 Position { get; private set; }
        private Vector2 velocity;

        private ParticleEmitterModel grenadeEmitter;

        public enum GrenadeType
        {
            Frag,
            Flashbang,
            Smoke,
            Incendiary
        }

        public enum State
        {
            NotThrown,
            Thrown,
            Landed
        }

        /// <summary>
        /// Throws the grenade
        /// </summary>
        /// <param name="angle"> Angle to throw the grenade from</param>
        /// <param name="velocity"> If the player is moving, add this to the initial velocity </param>
        public void Throw(float angle, Vector2 velocity)
        {

            // Get direction from vector
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            
            // Normalize the vector
            direction.Normalize();

            // If the player is already moving, this will increase the initial velocity of the
            // grenade.
            direction += velocity;

            // Set the velocity of this grenade
            this.velocity = direction;
        }

        /// <summary>
        /// If the grenade collides with the world, 
        /// </summary>
        public void CollidedWithWorld()
        {
            
        }
        

        public void Update(float gameTime)
        {
            
        }

        public void Draw(SpriteBatch sb)
        {
            
        }
    }
}
