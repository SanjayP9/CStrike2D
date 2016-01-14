using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycasting
{
    class RayEmitter
    {
        RayCastModel[] rays = new RayCastModel[200];

        public RayEmitter()
        {
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = new RayCastModel();
            }
        }

        public void Update(Vector2 emitLocal, Tile[,] tiles)
        {
            float angle;

            for (int i = 0; i < rays.Length; i++)
            {
                angle = MathHelper.ToRadians(i * (360 / rays.Length));

                rays[i].Update(emitLocal, new Vector2((float)Math.Cos(angle) * 2000f + emitLocal.X, (float)Math.Sin(angle) * 2000f + emitLocal.Y), tiles);
            }

        }

        public void Draw(SpriteBatch sb, Texture2D pixelTexture, Texture2D circleTexture)
        {
            foreach (RayCastModel i in rays)
            {
                i.View.Draw(sb, pixelTexture, circleTexture);
            }
        }
    }
}