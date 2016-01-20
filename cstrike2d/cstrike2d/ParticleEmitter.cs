// Author: Sanjay Paraboo
// File Name: ParticleModel.cs
// Project Name: Globabl Offensive ISU
// Creation Date: Dec 20th, 2015
// Modified Date: Jan 18th, 2016
// Description: Used to hold a list of particle sand set them to emit at a certain emit location.
//              It also runs all the Upd
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    public class ParticleEmitter
    {
        // These constants are used to store the life time of the particle system
        const float SMOKE_GRENADE_LIFETIME = 18000f;
        const float FIRE_GRENADE_LIFETIME = 7000f;
        const float REG_LIFETIME = 0.0f;

        // 
        const float SMOKE_UPDATE_FREQ = 50f;

        // Used to store a list of Particle Models
        public List<Particle> Particles { get; private set; }

        // Used to store the current ParticleType
        private Particle.ParticleTypes particleType;

        // Stores the emitter Vector2 Point
        private Vector2 emitVect;

        // Stores the life time of the particle system
        private float systemLifeTime = 0.0f;
        // Stores the current amount of time passed since the system up time
        private float systemUpTime = 0.0f;

        // Sets the update frequency of particle system
        private float updateFreq = 0.0f;


        /// <summary>
        /// Used to create an instance of ParticleEmitterModel. Depending on the particle type it will create different particle systems
        /// </summary>
        /// <param name="emitVect"></param>
        /// <param name="rand"></param>
        /// <param name="particleType"></param>
        public ParticleEmitter(Particle.ParticleTypes particleType)
        {
            this.particleType = particleType;

            // This switch statement initializes different Particles, system life times and more 
            // based on the particle type
            switch (particleType)
            {
                case Particle.ParticleTypes.Frag:
                    Particles = new List<Particle>(50);
                    systemLifeTime = 30.0f;
                    break;


                case Particle.ParticleTypes.Smoke:
                    Particles = new List<Particle>(50);
                    systemLifeTime = SMOKE_GRENADE_LIFETIME;
                    updateFreq = SMOKE_UPDATE_FREQ;
                    break;

                case Particle.ParticleTypes.GunSmoke:
                    Particles = new List<Particle>();
                    break;

                case Particle.ParticleTypes.Debris:
                    Particles = new List<Particle>();
                    break;

                case Particle.ParticleTypes.Shell:
                    Particles = new List<Particle>();
                    break;
            }
        }

        /// <summary>
        /// Runs update logic for the particle emitter
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            systemUpTime += gameTime;

            // add update time for smoke grenade

            for (int i = 0; i < Particles.Count; i++)
            {
                if (Particles[i] != null)
                {
                    Particles[i].Update(gameTime);

                    if ((particleType == Particle.ParticleTypes.Smoke) &&
                        (Particles[i].ParticleTransparency <= 0.0f) && (systemLifeTime > systemUpTime))
                    {
                        Particles[i].Respawn();
                    }

                    if (Particles[i].ParticleTransparency <= 0.0f)
                    {
                        Particles.RemoveAt(i);
                    }
                }
                else if ((Particles[i] == null) && (particleType == Particle.ParticleTypes.Smoke))
                {
                    Particles[i] = new Particle(emitVect, particleType,0f);
                }
            }

        }

        /// <summary>
        /// Starts the particle system by initializing a set amount of particles
        /// </summary>
        /// <param name="launchVect"> Specifies where the particles will be emitted from </param>
        public void Launch(Vector2 launchVect, float playerAngle)
        {
            // Updates emitter vector location
            this.emitVect = launchVect;

            // Sets the amount of launch particles based on the particle type
            int launchNumber = 0;

            switch (particleType)
            {
                case Particle.ParticleTypes.Frag:
                case Particle.ParticleTypes.Smoke:
                    launchNumber = 1;
                    break;

                case Particle.ParticleTypes.GunSmoke:
                case Particle.ParticleTypes.Debris:
                    launchNumber = 5;
                    break;

                case Particle.ParticleTypes.Shell:
                    launchNumber = 1;
                    break;
            }

            // When the launch method is called it will initialize a certain amount of particles
            //based on the Particle type
            for (int i = 0; i < launchNumber; i++)
            {
                Particles.Add(new Particle(launchVect, particleType, playerAngle));
            }
        }

        /// <summary>
        /// Draws Particle Emitter Model Data
        /// </summary>
        /// <param name="sb"> Passes through SpriteBatch instance in order to use Draw Method </param>
        /// <param name="particleImg"> Passes through the particle texture to be drawn </param>
        public void Draw(SpriteBatch sb, Texture2D particleImg)
        {
            // Cycles through every ParticleModel in Particles and draws it
            foreach (Particle particle in Particles)
            {
                particle.Draw(sb, particleImg);
            }
        }
    }
}
