using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    class ParticleEmitterModel
    {
        // These constants are used to store the life time of the particle system
        const float SMOKE_GRENADE_LIFETIME = 18000f;
        const float FIRE_GRENADE_LIFETIME = 7000f;
        const float REG_LIFETIME = 0.0f;

        // 
        const float SMOKE_UPDATE_FREQ = 50f;

        // Used to store a list of Particle Models
        public List<ParticleModel> Particles { get; private set; }

        // Stores an instance of ParticleEmitterView. This displays all of the model data
        public ParticleEmitterView View { get; private set; }

        // Used to store the current ParticleType
        private ParticleModel.ParticleTypes particleType;

        // Stores an instance of the Random Class
        private Random rand;

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
        public ParticleEmitterModel(Vector2 emitVect, Random rand, ParticleModel.ParticleTypes particleType)
        {
            this.particleType = particleType;
            this.rand = rand;
            this.emitVect = emitVect;

            // initializes the ParticleEmitter View and passes through this class
            View = new ParticleEmitterView(this);

            // This switch statement initializes different Particles, system life times and more 
            // based on the particle type
            switch (particleType)
            {
                case ParticleModel.ParticleTypes.Frag:
                    Particles = new List<ParticleModel>(50);
                    systemLifeTime = 30.0f;
                    break;

                case ParticleModel.ParticleTypes.Fire:
                    Particles = new List<ParticleModel>(50);
                    systemLifeTime = FIRE_GRENADE_LIFETIME;
                    break;

                case ParticleModel.ParticleTypes.Smoke:
                    Particles = new List<ParticleModel>(50);
                    systemLifeTime = SMOKE_GRENADE_LIFETIME;
                    updateFreq = SMOKE_UPDATE_FREQ;
                    break;

                case ParticleModel.ParticleTypes.GunSmoke:
                    Particles = new List<ParticleModel>();
                    break;

                case ParticleModel.ParticleTypes.Debris:
                    Particles = new List<ParticleModel>();
                    break;

                case ParticleModel.ParticleTypes.Shell:
                    Particles = new List<ParticleModel>();
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

                    if ((particleType == ParticleModel.ParticleTypes.Smoke || particleType == ParticleModel.ParticleTypes.Fire) &&
                        (Particles[i].ParticleTransparency <= 0.0f) && (systemLifeTime > systemUpTime))
                    {
                        Particles[i].Respawn();
                    }
                }
            }

        }

        /// <summary>
        /// Starts the particle system by initializing a set amount of particles
        /// </summary>
        /// <param name="launchVect"> Specifies where the particles will be emitted from </param>
        public void Launch(Vector2 launchVect)
        {
            // Sets the amount of launch particles based on the particle type
            int launchNumber = 0;

            switch (particleType)
            {
                case ParticleModel.ParticleTypes.Frag:
                case ParticleModel.ParticleTypes.Fire:
                case ParticleModel.ParticleTypes.Smoke:
                    launchNumber = 1;
                    break;

                case ParticleModel.ParticleTypes.GunSmoke:
                case ParticleModel.ParticleTypes.Debris:
                    launchNumber = 5;
                    break;

                case ParticleModel.ParticleTypes.Shell:
                    launchNumber = 1;
                    break;
            }

            // When the launch method is called it will initialize a certain amount of particles
            //based on the Particle type
            for (int i = 0; i < launchNumber; i++)
            {
                Particles.Add(new ParticleModel(launchVect, rand, particleType));
            }
        }
    }
}
