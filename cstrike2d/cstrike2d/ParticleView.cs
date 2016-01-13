using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class ParticleView
    {
        public ParticleModel Model { get; private set; }

        public ParticleView(ParticleModel model)
        {
            this.Model = model;
        }

        public void Draw(SpriteBatch sb, Texture2D particleImg)
        {
            sb.Draw(particleImg,
                    Model.ParticlePosition,
                    null,//Model.SourceRect,
                    Model.ParticleColor * Model.ParticleTransparency,
                    0f,
                    new Vector2(particleImg.Width * 0.5f, particleImg.Width * 0.5f),
                    Model.ParticleScale,
                    SpriteEffects.None,
                    0);
        }
    }
}
