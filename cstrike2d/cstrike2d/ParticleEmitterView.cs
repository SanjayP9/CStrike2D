// Author: Sanjay Paraboo
// File Name: ParticleEmitterView.cs
// Project Name: Global Offensive ISU
// Creation Date: Jan 1st, 2015
// Modified Date: Jan 18th, 2015
// Description:
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
        // Stores instance of ParticleEmitterModel
        private ParticleEmitterModel model;

        /// <summary>
        /// Used to create an instance of a ParticleEmitterView
        /// </summary>
        /// <param name="model"> Passes through the ParticleEmitterModel instance </param>
        public ParticleEmitterView(ParticleEmitterModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Draws Particle Emitter Model Data
        /// </summary>
        /// <param name="sb"> Passes through SpriteBatch instance in order to use Draw Method </param>
        /// <param name="particleImg"> Passes through the particle texture to be drawn </param>
        public void Draw(SpriteBatch sb, Texture2D particleImg)
        {
            // Cycles through every ParticleModel in Particles and draws it
            foreach(ParticleModel particle in model.Particles)
            {
                particle.View.Draw(sb, particleImg);
            }
        }
    }
}
