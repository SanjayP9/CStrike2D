using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class RayEmitter
    {
        const float ADJACENT_ANGLE = 0.01f;
        const float RAY_LENGTH = 1200f;//= Game1.SCREEN_WIDTH;

        private RayCast[] rays;
        private float angle;

        private Vector2[] rectVerticie;


        public RayEmitter(int numOfSolidTiles)
        {
            rays = new RayCast[numOfSolidTiles * 13];

            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = new RayCast();
            }

            rectVerticie = new Vector2[rays.Length];
        }

        public void Cast(Vector2 emitPos, Tile[,] tiles, Tile[] solidTiles)
        {
            // Rip code
            for (int i = 0; i < solidTiles.Length; i++)
            {
                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X + solidTiles[i].TileRect.Width, solidTiles[i].TileRect.Y + solidTiles[i].TileRect.Height));
                rays[i + (i * 12)].Update(emitPos, GetDirectionVect(angle), RAY_LENGTH, tiles, angle);
                rays[i + 1 + (i * 12)].Update(emitPos, GetDirectionVect(angle + ADJACENT_ANGLE), RAY_LENGTH, tiles, angle + ADJACENT_ANGLE);
                rays[i + 2 + (i * 12)].Update(emitPos, GetDirectionVect(angle - ADJACENT_ANGLE), RAY_LENGTH, tiles, angle - ADJACENT_ANGLE);

                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X, solidTiles[i].TileRect.Y + solidTiles[i].TileRect.Height));
                rays[i + 3 + (i * 12)].Update(emitPos, GetDirectionVect(angle), RAY_LENGTH, tiles, angle);
                rays[i + 4 + (i * 12)].Update(emitPos, GetDirectionVect(angle + ADJACENT_ANGLE), RAY_LENGTH, tiles, angle + ADJACENT_ANGLE);
                rays[i + 5 + (i * 12)].Update(emitPos, GetDirectionVect(angle - ADJACENT_ANGLE), RAY_LENGTH, tiles, angle - ADJACENT_ANGLE);


                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X + solidTiles[i].TileRect.Width, solidTiles[i].TileRect.Y));
                rays[i + 6 + (i * 12)].Update(emitPos, GetDirectionVect(angle), RAY_LENGTH, tiles, angle);
                rays[i + 7 + (i * 12)].Update(emitPos, GetDirectionVect(angle + ADJACENT_ANGLE), RAY_LENGTH, tiles, angle + ADJACENT_ANGLE);
                rays[i + 8 + (i * 12)].Update(emitPos, GetDirectionVect(angle - ADJACENT_ANGLE), RAY_LENGTH, tiles, angle - ADJACENT_ANGLE);


                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X, solidTiles[i].TileRect.Y));
                rays[i + 9 + (i * 12)].Update(emitPos, GetDirectionVect(angle), 1700f, tiles, angle);
                rays[i + 10 + (i * 12)].Update(emitPos, GetDirectionVect(angle + ADJACENT_ANGLE), 1700f, tiles, angle + ADJACENT_ANGLE);
                rays[i + 11 + (i * 12)].Update(emitPos, GetDirectionVect(angle - ADJACENT_ANGLE), 1700f, tiles, angle - ADJACENT_ANGLE);
            }

        }

        public void Draw(SpriteBatch sb, Texture2D pixelTexture, Texture2D circleTexture, SpriteFont font)
        {
            foreach (RayCast i in rays)
            {
             //   i.View.Draw(sb, pixelTexture, circleTexture);
            }

            // Insert polygon filling


            sb.DrawString(font, "Ray Count: " + rays.Length, new Vector2(1200 - (font.MeasureString("Ray Count: " + rays.Length).X), 700 - (font.MeasureString("Ray Count: " + rays.Length).Y)), Color.Black);
        }

        public float GetAngle(Vector2 vect1, Vector2 vect2)
        {
            return (float)(Math.Atan2(vect2.Y - vect1.Y, vect2.X - vect1.X));
        }

        public Vector2 GetDirectionVect(float angle)
        {
            return (new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
        }

        public void UpdateVerticies()
        {
            for (int i = 0; i < rays.Length; i++)
            {
                rectVerticie[i] = rays[i].CollisionPos;
            }
        }

    }
}