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
    class Particle
    {
        // Constants used to store to control the update frequency
        private const float SMOKE_UPDATE_FREQ = 50.0f;

        private float updateTime = 0.0f;
        private float updateFreq = 0.0f;

        // Used to store the current particle position and the original emit vector
        private Vector2 ParticlePosition;
        private Vector2 emitVect;

        // Properties used for drawing
        // Controls transparency of particle texture
        public float ParticleTransparency { get; private set; }
        //Scales the particles texture size 
        private float ParticleScale;
        //Controls particle texture color overlay
        private Color particleColor;
        // Used to change the rotation of the texture when drawing
        public float Rotation { get; private set; }


        // Records how long the particle has been acitve for
        private float ParticleLifeTime;

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
            Debris,
            Shell
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
        public Particle(Vector2 emitVect,ParticleTypes particleType, float playerAngle)
        {
            this.ParticlePosition = emitVect;
            this.emitVect = emitVect;
            this.Type = particleType;

            switch (Type)
            {
                case ParticleTypes.Frag:

                    particleColor = new Color(250, 250, 0);


                    particleDirection = CalcDirectionVect(CStrike2D.Rand.Next(0, 361));

                    if (CStrike2D.Rand.Next(0, 101) < 10)
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
                    particleColor = Color.Yellow;

                    particleVelocity = 0.7f;

                    ParticleTransparency = 1.0f;
                    ParticleScale = 0.3f;
                    particleDirection = CalcDirectionVect(CStrike2D.Rand.Next(0, 361));

                    break;

                case ParticleTypes.Smoke:
                    updateFreq = SMOKE_UPDATE_FREQ;

                    particleColor = Color.Gray;
                    particleDirection = CalcDirectionVect(CStrike2D.Rand.Next(0, 361));


                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    ParticleScale = 1.5f;

                    break;

                case ParticleTypes.GunSmoke:
                    particleColor = new Color(120, 120, 120);

                    particleDirection = CalcDirectionVect((float)(-Math.PI * 0.5f));

                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    ParticleScale = 0.01f;
                    break;

                case ParticleTypes.Debris:
                    particleColor = Color.White;
                    break;

                case ParticleTypes.Shell:
                    particleColor = Color.White;
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
        public void Update(float gameTime)
        {
            // Adds elapsed game time to 
            ParticleLifeTime += gameTime;
            updateTime += gameTime;
            updateTime = 0.0f;

            // Adds the direction multiplied by the speed to the current particle position
            ParticlePosition += particleDirection * particleVelocity;

            if ((ParticleTransparency <= 0.0f) && (Type == ParticleTypes.Smoke || Type == ParticleTypes.Fire))
            {
                Respawn();
            }


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
                    ParticleTransparency -= 0.007f;
                    break;

                case ParticleTypes.GunSmoke:
                    ParticleScale += 0.005f;
                    ParticleTransparency -= 0.07f;
                    break;

                case ParticleTypes.Debris:
                    if (!(particleVelocity <= 0.0f))
                    {
                        particleVelocity -= 0.25f;
                    }
                    else
                    {
                        particleVelocity = 0.0f;
                    }

                    if (ParticleLifeTime >= 3000f)
                    {
                        ParticleTransparency -= 0.05f;
                    }
                    break;
            }

        }

        /// <summary>
        /// Used to respawn particles back at their emit vectors
        /// </summary>
        public void Respawn()
        {
            ParticleTransparency = 1.0f;
            ParticlePosition = emitVect;
            ParticleLifeTime = 0.0f;

            switch (Type)
            {
                case ParticleTypes.Frag:
                    particleColor = new Color(250, 250, 0);
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

        public Vector2 CalcDirectionVect(float angle)
        {
            return new Vector2((float)(Math.Cos(angle)), (float)(Math.Sin(angle)));
        }


        public Color ReturnColor()
        {
            return particleColor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="particleImg"></param>
        public void Draw(SpriteBatch sb, Texture2D particleImg)
        {
            sb.Draw(particleImg,
                    ParticlePosition,
                    null,//Model.SourceRect,
                    ReturnColor() * ParticleTransparency,
                    0f,
                    new Vector2(particleImg.Width * 0.5f, particleImg.Width * 0.5f),
                    ParticleScale,
                    SpriteEffects.None,
                    0);
        }
    }
}
