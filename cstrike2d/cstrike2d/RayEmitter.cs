// Author: Sanjay Paraboo
// File Name: RayEmitter.cs
// Project Name: Global Offensive
// Creation Date: Jan 14th, 2015
// Modified Date: Jan 19th, 2016
// Description: Handles casting multiple rays which are used in drawing a visiblity polygon
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CStrike2D
{
    class RayEmitter
    {
        const float ADJACENT_ANGLE = 0.01f;
        const float RAY_LENGTH = 1200f;

        public RayCast[] Rays { get; private set; }
        private float angle;

        private Vector2[] rectVerticie;


        public RayEmitter()
        {
            Rays = new RayCast[1024];

            for (int i = 0; i < Rays.Length; i++)
            {
                Rays[i] = new RayCast();
            }

            rectVerticie = new Vector2[Rays.Length];
        }

        public void Cast(Vector2 emitPos, Tile[,] tiles, Tile[] solidTiles, Rectangle mapArea)
        {
            
            for (int i = 0; i < solidTiles.Length; i++)
            {
                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X + solidTiles[i].TileRect.Width, solidTiles[i].TileRect.Y + solidTiles[i].TileRect.Height));
                Rays[i + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle);
                Rays[i + 1 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle + ADJACENT_ANGLE);
                Rays[i + 2 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle - ADJACENT_ANGLE);

                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X, solidTiles[i].TileRect.Y + solidTiles[i].TileRect.Height));
                Rays[i + 3 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle);
                Rays[i + 4 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle + ADJACENT_ANGLE);
                Rays[i + 5 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle - ADJACENT_ANGLE);


                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X + solidTiles[i].TileRect.Width, solidTiles[i].TileRect.Y));
                Rays[i + 6 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle);
                Rays[i + 7 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle + ADJACENT_ANGLE);
                Rays[i + 8 + (i * 12)].Update(emitPos, RAY_LENGTH, tiles, mapArea, angle - ADJACENT_ANGLE);


                angle = GetAngle(emitPos, new Vector2(solidTiles[i].TileRect.X, solidTiles[i].TileRect.Y));
                Rays[i + 9 + (i * 12)].Update(emitPos, 1700f, tiles, mapArea, angle);
                Rays[i + 10 + (i * 12)].Update(emitPos, 1700f, tiles, mapArea, angle + ADJACENT_ANGLE);
                Rays[i + 11 + (i * 12)].Update(emitPos, 1700f, tiles, mapArea, angle - ADJACENT_ANGLE);
            }
            
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
            for (int i = 0; i < Rays.Length; i++)
            {
                rectVerticie[i] = Rays[i].CollisionPos;
            }
        }

    }
}