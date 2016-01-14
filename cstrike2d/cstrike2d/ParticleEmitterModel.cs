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
        // 
        const float SMOKE_GRENADE_LIFETIME = 18000f;
        const float FIRE_GRENADE_LIFETIME = 7000f;
        const float REG_LIFETIME = 1000f;

        public ParticleModel[] Particles { get; private set; }

        public ParticleEmitterView View { get; private set; }

        private ParticleModel.ParticleTypes particleType;

        private Random rand;

        private Vector2 emitVect;

        private float systemLifeTime = 0.0f;
        private float systemUpTime = 3000f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emitVect"></param>
        /// <param name="rand"></param>
        /// <param name="particleType"></param>
        public ParticleEmitterModel(Vector2 emitVect, Random rand, ParticleModel.ParticleTypes particleType)
        {
            this.particleType = particleType;
            this.rand = rand;
            this.emitVect = emitVect;

            View = new ParticleEmitterView(this);

            switch (particleType)
            {
                case ParticleModel.ParticleTypes.Frag:
                    Particles = new ParticleModel[50];
                    systemLifeTime = 30.0f;
                    break;

                case ParticleModel.ParticleTypes.Fire:
                    Particles = new ParticleModel[50];
                    systemLifeTime = FIRE_GRENADE_LIFETIME;
                    break;

                case ParticleModel.ParticleTypes.Smoke:
                    Particles = new ParticleModel[50];
                    systemLifeTime = SMOKE_GRENADE_LIFETIME;
                    break;

                case ParticleModel.ParticleTypes.GunSmoke:
                    break;

                case ParticleModel.ParticleTypes.Debris:
                    break;

                case ParticleModel.ParticleTypes.Shell:
                    break;
            }
        }


        public void Update(float gameTime)
        {
            systemUpTime += gameTime;


        }
    }
}
