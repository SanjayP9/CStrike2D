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
        public ParticleModel[] Particles { get; private set; }

        public ParticleEmitterView View { get; private set; }

        private ParticleModel.ParticleTypes particleType;

        private Random rand;

        private Vector2 emitVect;

        public ParticleEmitterModel(Vector2 emitVect, Random rand, ParticleModel.ParticleTypes particleType)
        {
            this.particleType = particleType;
            this.rand = rand;
            this.emitVect = emitVect;

            View = new ParticleEmitterView(this);

            switch (particleType)
            {
                case ParticleModel.ParticleTypes.Frag:
                    break;

                case ParticleModel.ParticleTypes.Fire:
                    break;

                case ParticleModel.ParticleTypes.Smoke:
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

        }
    }
}
