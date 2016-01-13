// Author: Sanjay Paraboo
// File Name: ParticleModel.cs
// Project Name: Globabl Offensice ISU
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 18th, 2015
// Description: Used to implement particle effects. This class stores all logic for
//              the particle effects
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class ParticleModel
    {
        // Used to store the current particle position and the original emit vector
        public Vector2 ParticlePosition { get; protected set; }
        private Vector2 emitVect;

        // Properties used for drawing
        // Controls transparency of particle texture
        public float ParticleTransparency { get; private set; }
        //Scales the particles texture size 
        public float ParticleScale { get; private set; }
        //Controls particle texture color overlay
        public Color ParticleColor { get { return particleColor; } private set; }
        private Color particleColor;
        // Stores an instance of particle model. Used to draw particles
        public ParticleView View { get; private set; }
        //Stores the source rectangle. Used when drawing
        public Rectangle SourceRect { get; private set; }

        // Records how long the particle has been acitve for
        public float ParticleLifeTime { get; private set; }

        // Stores the direction vector of the particle
        private Vector2 particleDirection;
        // Used to store the particles velocity. 
        private float particleVelocity;


        /// <summary>
        /// Lists all types of particles in the game
        /// </summary>
        public enum ParticleTypes
        {
            Frag,
            Fire,
            Smoke,
            GunSmoke,
            Debris
        };
        // Stores the current particle type
        public ParticleTypes Type { get; private set; }

        /// <summary>
        /// Creates an instance of ParticleModel and initializes the ParticleView
        /// </summary>
        /// <param name="emitVect"> 
        /// Passes through the particle emit vector 
        /// </param>
        /// <param name="rand"> 
        /// Passes through instance of a Random in order 
        /// to create random integers
        /// </param>
        protected ParticleModel(Vector2 emitVect, Random rand, ParticleTypes particleType)
        {
            this.ParticlePosition = emitVect;
            this.emitVect = emitVect;
            this.Type = particleType;

            View = new ParticleView(this);

            switch (Type)
            {
                case ParticleTypes.Frag:

                    particleColor = new Color(250, 250, 0);

                    particleDirection = CalcDirectionVect(rand.Next(0, 361));

                    if (rand.Next(0, 101) < 10)
                    {
                        particleVelocity = 5.0f;
                    }
                    else
                    {
                        particleVelocity = 3.0f;
                    }

                    ParticleScale = 0.2f;
                    ParticleTransparency = 1.0f;


                    break;
                case ParticleTypes.Fire:
                    break;
                case ParticleTypes.Smoke:

                    particleColor = Color.Gray;
                    particleDirection = CalcDirectionVect(rand.Next(0, 361));


                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    ParticleScale = 1.5f;

                    break;
                case ParticleTypes.GunSmoke:
                    break;
                case ParticleTypes.Debris:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Used to run update logic for a particle such as updating location,
        /// transparency, color and other properties
        /// </summary>
        /// <param name="gameTime"> Passes through gameTime in order to record elasped time </param>
        public virtual void Update(float gameTime)
        {
            // Adds elapsed game time to 
            ParticleLifeTime += gameTime;

            // Adds the direction multiplied by the speed to the current particle position
            ParticlePosition += particleDirection * particleVelocity;

            switch (Type)
            {
                case ParticleTypes.Frag:

                    if (particleColor.G > 165)
                    {
                        particleColor.G -= 17;
                    }
                    else
                    {
                        if (particleColor.R > 0)
                        {
                            particleColor.R -= 10;
                        }
                        if (particleColor.G > 0)
                        {
                            particleColor.G -= 15;
                        }
                    }


                    ParticleTransparency -= 0.02f;
                    particleVelocity -= 0.025f;

                    if (ParticleScale < 0.4f)
                    {
                        ParticleScale += 0.01f;
                    }

                    break;
                case ParticleTypes.Fire:
                    break;
                case ParticleTypes.Smoke:
                    break;
                case ParticleTypes.GunSmoke:
                    break;
                case ParticleTypes.Debris:
                    break;
            }

        }

        /// <summary>
        /// Used to respawn particles back at their emit vectors
        /// </summary>
        public virtual void Respawn()
        {
            ParticleTransparency = 1.0f;
            ParticlePosition = emitVect;
            ParticleLifeTime = 0.0f;

            switch (Type)
            {
                case ParticleTypes.Frag:
                    ParticleColor = new Color(250, 250, 0);
                    break;
                case ParticleTypes.Fire:
                    break;
                case ParticleTypes.Smoke:
                    break;
                case ParticleTypes.GunSmoke:
                    break;
                case ParticleTypes.Debris:
                    break;
                default:
                    break;
            }
        }

        public Vector2 CalcDirectionVect(int angle)
        {
            return new Vector2((float)(Math.Cos(angle)), (float)(Math.Sin(angle)));
        }
    }
}
