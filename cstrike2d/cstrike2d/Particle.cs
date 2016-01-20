// Author: Sanjay Paraboo
// File Name: ParticleModel.cs
// Project Name: Globabl Offensive ISU
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 18th, 2016
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
    public class Particle
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
        private float particleScale;
        //Controls particle texture color overlay
        private Color particleColor;
        // Used to change the rotation of the texture when drawing
        public float rotation;


        // Records how long the particle has been acitve for
        private float particleLifeTime;

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
            Smoke,
            GunSmoke,
            Debris,
            Shell
        };
        // Stores the current particle type
        public ParticleTypes Type { get; private set; }

        
        /// <summary>
        /// Used to create an instance of a particle and based on its particle type it will have certain attributes
        /// </summary>
        /// <param name="emitVect"> Passes through the emitter vector2 </param>
        /// <param name="particleType"> Specifies what particle to create </param>
        /// <param name="playerAngle"> Specifies what angle the player is at </param>
        public Particle(Vector2 emitVect,ParticleTypes particleType, float playerAngle)
        {
            // Sets class level variables to ones retrieved in parameter
            this.ParticlePosition = emitVect;
            this.emitVect = emitVect;
            this.Type = particleType;

            // Bassed upon the Particle type the particle will have a specific color, velocity, scale
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

                    particleScale = 0.2f;
                    ParticleTransparency = 1.0f;


                    break;

                case ParticleTypes.Smoke:
                    updateFreq = SMOKE_UPDATE_FREQ;

                    particleColor = Color.Gray;
                    particleDirection = CalcDirectionVect(CStrike2D.Rand.Next(0, 361));


                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    particleScale = 1.5f;

                    break;

                case ParticleTypes.GunSmoke:
                    particleColor = new Color(120, 120, 120);

                    particleDirection = CalcDirectionVect(playerAngle);

                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    particleScale = 0.01f;

                    rotation = playerAngle;
                    break;

                case ParticleTypes.Debris:
                    particleColor = Color.White;
                    break;

                case ParticleTypes.Shell:
                    particleColor = Color.White;

                    particleScale = 0.1f;
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
            particleLifeTime += gameTime;
            updateTime += gameTime;
            updateTime = 0.0f;

            // Adds the direction multiplied by the speed to the current particle position
            ParticlePosition += particleDirection * particleVelocity;

            if ((ParticleTransparency <= 0.0f) && (Type == ParticleTypes.Smoke))
            {
                Respawn();
            }


            // Based on the type of particle it will have different properties such as scale, its color transparency and etc
            switch (Type)
            {
                case ParticleTypes.Frag:

                    // Changes RGB colors based on its current values
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

                    // Decrements the transparency, velocity and scale
                    ParticleTransparency -= 0.02f;
                    particleVelocity -= 0.025f;

                    if (particleScale < 0.4f)
                    {
                        particleScale += 0.01f;
                    }
                    break;

                case ParticleTypes.Smoke:
                    // Decrements the alpha
                    ParticleTransparency -= 0.007f;
                    break;

                case ParticleTypes.GunSmoke:
                    // Increases scale and decrements alpha
                    particleScale += 0.005f;
                    ParticleTransparency -= 0.07f;
                    break;

                case ParticleTypes.Debris:
                    // If the velocity 
                    if (!(particleVelocity <= 0.0f))
                    {
                        particleVelocity -= 0.25f;
                    }
                    else
                    {
                        particleVelocity = 0.0f;
                    }

                    if (particleLifeTime >= 3000f)
                    {
                        ParticleTransparency -= 0.05f;
                    }
                    break;

                case ParticleTypes.Shell:
                    if (!(particleVelocity <= 0.0f))
                    {
                        particleVelocity -= 0.25f;
                    }
                    else
                    {
                        particleVelocity = 0.0f;
                    }
                    if (particleLifeTime >= 1500f)
                    {
                        ParticleTransparency -= 0.05f;
                    }

                    rotation += 0.01f;
                    break;
            }

        }

        /// <summary>
        /// Used to respawn particles back at their emit vectors
        /// </summary>
        public void Respawn()
        {
            // When the particle respawns it resets the transparency, position and resets the particle lifetime
            ParticleTransparency = 1.0f;
            ParticlePosition = emitVect; 
            particleLifeTime = 0.0f;


            // if the grenade is a frag it resets the color
            if (Type == ParticleTypes.Frag)
            {
                particleColor = new Color(250, 250, 0);
            }
        }

        /// <summary>
        /// Returns the direction vector  given an angle
        /// </summary>
        /// <param name="angle"> Passes through an angle in radians </param>
        /// <returns></returns>
        public Vector2 CalcDirectionVect(float angle)
        {
            return new Vector2((float)(Math.Cos(angle)), (float)(Math.Sin(angle)));
        }


        /// <summary>
        /// Draws the particle instance onto the screen
        /// </summary>
        /// <param name="sb"> Passes through spritebatch in order to use its draw commands </param>
        /// <param name="particleImg"> passes through the particle texture that will be drawn </param>
        public void Draw(SpriteBatch sb, Texture2D particleImg)
        {
            sb.Draw(particleImg,                            // Uses particle image as Texture2D
                    ParticlePosition,                       // Sets it at the ParticlePosition Vector2
                    null,                                   // No Source Rectangle used
                    particleColor * ParticleTransparency,   // Uses the colour and transparency calculated in the update code 
                    rotation,                               // Uses roation from update logic
                    new Vector2(particleImg.Width * 0.5f,   // Centres the texture at the middle of the texture
                                particleImg.Width * 0.5f),
                    particleScale,                          // Scales it using float scale calulated in update logic
                    SpriteEffects.None,                     // No SpriteEffects used
                    0);                                     // Drawn on layer 0
        }
    }
}
