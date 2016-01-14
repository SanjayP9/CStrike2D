using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycasting
{
    class RayCastView
    {
        public RayCastModel Model { get; private set; }

        public RayCastView(RayCastModel model)
        {
            this.Model = model;
        }

        public void Draw(SpriteBatch sb, Texture2D pixelTexture, Texture2D circleTexture)
        {


            sb.Draw(pixelTexture,
                    new Rectangle((int)Model.RayPos.X, (int)Model.RayPos.Y, (int)Model.DifferenceVect.Length(), 2),
                    null,
                    Color.Red,
                    Model.AngleToIntersect,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);

            sb.Draw(circleTexture, new Vector2(Model.IntersectPos.X - (circleTexture.Width * 0.007f * 0.5f), Model.IntersectPos.Y - (circleTexture.Height * 0.007f * 0.5f)), null, Color.Black, 0.0f, Vector2.Zero, 0.007f, SpriteEffects.None, 0);
        }
    }
}
