﻿// Author: Sanjay Paraboo
// File Name: ParticleModel.cs
// Project Name: Global Offensive
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 20th, 2016
// Description: Used to implement particle effects. This class stores all logic for
//              the particle effects
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CStrike2D
{
    public class Particle
    {
        // Constants used to store to control the update frequency
        private const float SMOKE_UPDATE_FREQ = 50.0f;

        //  Used to specify update intercals of certain particles
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
        private float rotation;
        // Rate at which the rotation changes;
        private float rotationChange;

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
            Shell,
            Blood
        };
        // Stores the current particle type
        public ParticleTypes Type { get; private set; }


        /// <summary>
        /// Used to create an instance of a particle and based on its particle type it will have certain attributes
        /// </summary>
        /// <param name="emitVect"> Passes through the emitter vector2 </param>
        /// <param name="particleType"> Specifies what particle to create </param>
        /// <param name="playerAngle"> Specifies what angle the player is at </param>
        public Particle(Vector2 emitVect, ParticleTypes particleType, float playerAngle)
        {
            // Sets class level variables to ones retrieved in parameter
            this.ParticlePosition = emitVect;
            this.emitVect = emitVect;
            this.Type = particleType;

            // Bassed upon the Particle type the particle will have a specific color, velocity, scale
            switch (Type)
            {
                case ParticleTypes.Frag:

                    // Particle color starts off as yellow for a frag grenade
                    particleColor = new Color(250, 250, 0);
                    // Direction is randomly genarated in a 360 degree radius
                    particleDirection = CalcDirectionVect(CStrike2D.Rand.Next(0, 361));

                    // 10% chance of having a high velocity particle
                    if (CStrike2D.Rand.Next(0, 101) < 10)
                    {
                        particleVelocity = 5.0f;
                    }
                    else
                    {
                        particleVelocity = 3.0f;
                    }

                    // Scales the intial size to 20% and sets the initial transparency to 100%
                    particleScale = 0.2f;
                    ParticleTransparency = 1.0f;
                    break;

                case ParticleTypes.Smoke:

                    // Updates the smoke particle every SMOKE_UPDATE_FREQ
                    updateFreq = SMOKE_UPDATE_FREQ;

                    // Sets initial color to Cray and randomly generates a direciton from a 360 degree radius
                    particleColor = Color.Gray;
                    particleDirection = CalcDirectionVect(CStrike2D.Rand.Next(0, 361));

                    // Initial velocity and transparency set to 1
                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    // Initla particle scale is 150%
                    particleScale = 1.5f;
                    break;

                case ParticleTypes.GunSmoke:
                    // Sets Color to light gray
                    particleColor = new Color(120, 120, 120);
                    // Gets the direction from the direciton of the player barrel
                    particleDirection = CalcDirectionVect(playerAngle);

                    // Has default values for velocity and transparency and scale is set to 1%
                    particleVelocity = 1.0f;
                    ParticleTransparency = 1.0f;
                    particleScale = 0.01f;

                    // Matches the rotation to the players angle
                    rotation = playerAngle;
                    break;

                case ParticleTypes.Debris:
                    // Sets intial color to transparent
                    particleColor = Color.White;
                    break;

                case ParticleTypes.Shell:
                    // No overlay color
                    particleColor = Color.White;

                    // randomly gets a direction from a 52 degree radius that perpendicular to the player angle
                    particleDirection = CalcDirectionVect(playerAngle + (((CStrike2D.Rand.Next(0, 52) - 52) / 100f)));
                    ParticleTransparency = 1.0f;
                    // random velocity from 2.5 to 3.5
                    particleVelocity = CStrike2D.Rand.Next(25, 35) / 10f;

                    // Texture scales to 2%
                    particleScale = 0.02f;

                    // Constant rotation change id randomly generated so each shell has a unique movement
                    rotationChange = 0.6f * (float)(CStrike2D.Rand.NextDouble() * (CStrike2D.Rand.Next(0, 2) - 1f));
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
            // Adds elapsed game time to update time and gametime
            particleLifeTime += gameTime;
            updateTime += gameTime;

            // Adds the direction multiplied by the speed to the current particle position
            ParticlePosition += particleDirection * particleVelocity;

            if ((ParticleTransparency <= 0.0f) && (Type == ParticleTypes.Smoke))
            {
                // If the current particle type is a smoke set it to respawn when it dissapears
                Respawn();
            }


            // Based on the type of particle it will have different properties such as scale, its color transparency and etc
            switch (Type)
            {
                case ParticleTypes.Frag:

                    // Changes RGB colors based on its current values it will go from yellow to orange to red to black
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

                    // Decrements the transparency, velocity and increases scale
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
                    // If the velocity is not less than or equal to 0 it wil decrement the velocity
                    if (!(particleVelocity <= 0.0f))
                    {
                        particleVelocity -= 0.25f;
                    }
                    else// if the velocity is under 0 sets it back to zero
                    {
                        particleVelocity = 0.0f;
                    }
                    
                    // After 3 secodns the debrris dissapears
                    if (particleLifeTime >= 3.0f)
                    {
                        ParticleTransparency -= 0.05f;
                    }
                    break;

                case ParticleTypes.Shell:

                    // If the velocity is more than zero it derements the velocity and rotates the texture
                    if (!(particleVelocity <= 0.0f))
                    {
                        particleVelocity -= 0.16f;
                        rotation += rotationChange;
                    }
                    else // If the velocity is less than 0 it sets it to 0
                    {
                        particleVelocity = 0.0f;
                    }

                    // If the shell lasts more than 0.35 seconds it will start to disapear
                    if (particleLifeTime >= 0.35f)
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
                                particleImg.Height * 0.5f),
                    particleScale,                          // Scales it using float scale calulated in update logic
                    SpriteEffects.None,                     // No SpriteEffects used
                    0);                                     // Drawn on layer 0
        }
    }
}
