using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class RayCastView
    {
        //Newest Version
        //public RayCastModel Model { get; private set; }

        //public RayCastView(RayCastModel model)
        //{
        //    this.Model = model;
        //}

        //public void Draw(SpriteBatch sb, Texture2D pixelTexture, Texture2D circleTexture)
        //{
        //    sb.Draw(pixelTexture,
        //            new Rectangle((int)Model.EmitPos.X, (int)Model.EmitPos.Y, (int)Model.GetRayLength(), 2),
        //            null,
        //            Model.RayColor,
        //            Model.Angle,
        //            Vector2.Zero,
        //            SpriteEffects.None,
        //            0);

        //    sb.Draw(circleTexture, new Vector2(Model.CollisionPos.X - (circleTexture.Width * 0.0045f), Model.CollisionPos.Y - (circleTexture.Height * 0.0045f)), null, Color.Black, 0.0f, Vector2.Zero, 0.007f, SpriteEffects.None, 0);
        //}
    }
}
