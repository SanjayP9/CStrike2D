// Author: Sanjay Paraboo
// File Name: ClientGrenade.cs
// Project Name: Global Offensive
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 18th, 2016
// Description: Used to create an instance of a grenade in the player class
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    class ClientGrenade
    {
        // Stores an instance of assests
        private Assets assets;

        // Vectors ans floats below are used to store th ecurrent position of the grenade,
        // its speed and the direction its moving at
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        private float velocity;

        // Stores an instance of Particle emitter for the grenade particle effects
        private ParticleEmitter grenadeEmitter;


        /// <summary>
        /// Used to record what current state of the grenade is
        /// </summary>
        public enum GrenadeStates
        {
            Empty,
            InPossession,
            Thrown,
            Exploding
        }

        // Enums below store what the grenade type is (ex: flash, smoke or frag) and records
        // the current state of the grenade
        public Particle.ParticleTypes NadeType { get; private set; }
        public GrenadeStates State { get; private set; }


        /// <summary>
        /// Creates an instance of the client grenade
        /// </summary>
        /// <param name="assets"></param>
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
        /// If the grenade collides with the world, it bouces it off the solid object
        /// </summary>
        public void CollidedWithWorld()
        {

        }


        /// <summary>
        /// Runs the update logic of the grenade based on the current state
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            switch (State)
            {
                // When the grenade state is empty or in possesion no logic needs to be run
                case GrenadeStates.Empty:
                    break;

                case GrenadeStates.InPossession:
                    break;

                case GrenadeStates.Thrown:

                    // Updates position of the nade when thrown
                    Position += Direction * velocity;

                    // Decrements the velocity
                    if (velocity >= 0.0f)
                    {
                        velocity -= 0.05f;
                    }


                    // When the velocity slows down to 0 it will explode
                    if (velocity <= 0.0f)
                    {
                        velocity = 0.0f;
                        Explode();
                    }
                    break;


                case GrenadeStates.Exploding:
                    // When the grenade is exploding it will update the particle emitter
                    grenadeEmitter.Update(gameTime);

                    break;
            }
        }

        /// <summary>
        /// Runs this method when the grenade needs to explode
        /// </summary>
        public void Explode()
        {
            // Creates an instance of a ParticleEmitter
            grenadeEmitter = new ParticleEmitter(NadeType);
            grenadeEmitter.Launch(Position, 0f);
        }


        /// <summary>
        /// Method runs when the player picks up a grenade
        /// </summary>
        /// <param name="grenadePickup"> Specifies which grenade is picked up </param>
        public void PickupNade(Particle.ParticleTypes grenadePickup)
        {
            NadeType = grenadePickup;
        }

        /// <summary>
        /// Draws the grenade based upon the current grenade type
        /// </summary>
        /// <param name="sb"> Passes through SpriteBatch instance in order to use Draw method </param>
        public void Draw(SpriteBatch sb)
        {
            // Draws the grenade particles based upon the current nade type
            switch (NadeType)
            {
                case Particle.ParticleTypes.Frag:
                    grenadeEmitter.Draw(sb, assets.ParticleTexture);
                    break;
                    
                case Particle.ParticleTypes.Smoke:
                    grenadeEmitter.Draw(sb, assets.SmokeParticle);
                    break;
            }
        }
    }
}
