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

        private Assets assets;

        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        private float velocity;

        private ParticleEmitter grenadeEmitter;


        public enum GrenadeStates
        {
            Empty,
            InPossession,
            Thrown,
            Exploding
        }

        public Particle.ParticleTypes NadeType { get; private set; }
        public GrenadeStates State { get; private set; }


        public ClientGrenade(Assets assets)
        {
            this.assets = assets;
        }

        /// <summary>
        /// Throws the grenade
        /// </summary>
        /// <param name="angle"> Angle to throw the grenade from</param>
        /// <param name="velocity"> If the player is moving, add this to the initial velocity </param>
        public void Throw(float angle, float velocity)
        {
            // Updates the class velocity
            this.velocity = velocity;

            // Changes the current nade state
            State = GrenadeStates.Thrown;

            // Get direction from vector
            Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            // Normalize the vector
            Direction.Normalize();
        }

        /// <summary>
        /// If the grenade collides with the world, 
        /// </summary>
        public void CollidedWithWorld()
        {

        }


        public void Update(float gameTime)
        {
            switch (State)
            {
                case GrenadeStates.Empty:
                    break;

                case GrenadeStates.InPossession:
                    break;

                case GrenadeStates.Thrown:
                    // Updates position of the nade
                    Position += Direction * velocity;

                    // Decrements the velocity
                    if (velocity >= 0.0f)
                    {
                        velocity -= 0.05f;
                    }

                    if (velocity <= 0.0f)
                    {
                        velocity = 0.0f;
                        Explode();
                    }

                    break;


                case GrenadeStates.Exploding:
                    // update emitter
                    grenadeEmitter.Update(gameTime);

                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Explode()
        {
            grenadeEmitter = new ParticleEmitter(Position, NadeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nadeType"></param>
        public void PickupNade(Particle.ParticleTypes nadeType)
        {
            this.NadeType = nadeType;
            State = GrenadeStates.InPossession;
        }

        public void Draw(SpriteBatch sb)
        {
            switch (NadeType)
            {
                case Particle.ParticleTypes.Frag:
                    grenadeEmitter.Draw(sb, assets.ParticleTexture);
                    break;

                case Particle.ParticleTypes.Fire:
                    grenadeEmitter.Draw(sb, assets.SmokeParticle);
                    break;

                case Particle.ParticleTypes.Smoke:
                    grenadeEmitter.Draw(sb, assets.SmokeParticle);
                    break;
            }
        }
    }
}
