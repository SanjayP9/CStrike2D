using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    class ParticleEmitterView
    {
        private ParticleEmitterModel model;

        public ParticleEmitterView(ParticleEmitterModel model)
        {
            this.model = model;
        }

        public void Draw(SpriteBatch sb, Texture2D particleImg)
        {
            foreach (ParticleModel i in model.Particles)
            {
                i.View.Draw(sb, particleImg);
            }
        }
    }
}
