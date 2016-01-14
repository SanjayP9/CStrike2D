using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycasting
{
    class RayCastModel
    {
        public RayCastView View { get; private set; }

        //private bool doesRayCollide;
        //private Vector2 position;


        public Vector2 RayPos { get; private set; }
        public Vector2 DestinationPos { get; private set; }

        public Vector2 IntersectPos { get; private set; }
        public float AngleToIntersect { get; private set; }

        public Vector2 DifferenceVect { get; private set; }

        public RayCastModel()
        {
           View = new RayCastView(this);
        }


        public void Update(Vector2 rayPos, Vector2 destinationPos, Tile[,] tiles)
        {
            this.RayPos = rayPos;
            this.DestinationPos = destinationPos;

            IntersectPos = FindIntersection(GetPointsOnRay(rayPos, destinationPos, tiles), tiles);

            //IntersectPos = DestinationPos;//temp

            DifferenceVect = IntersectPos - RayPos;
            AngleToIntersect = (float)(Math.Atan2(DifferenceVect.Y, DifferenceVect.X));

        }

        /// <summary>
        /// Uses the Bresham Line algorithm to find each of the points on a line segment
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public Vector2[] GetPointsOnRay(Vector2 point1, Vector2 point2, Tile[,] tiles)
        {
            List<Vector2> pointsOnRay = new List<Vector2>();


            // Checks to see if the slope is >45 degrees or <45 degrees. Bu checkin to see if the y values
            // are larger than the x values
            bool isSlopeSteep = (Math.Abs(point2.Y - point1.Y) > Math.Abs(point2.X - point1.X));



            //Swaps Vectors if the slope is steep because
            if (isSlopeSteep)
            {
                point1 = SwapVectorCoordinates(point1);
                point2 = SwapVectorCoordinates(point2);
            }

            //temp
            bool isLineRightToLeft = (point1.X > point2.X);
            //temp

            // Checks to see if the points go from left to right if not it swaps the vectors
            if (isLineRightToLeft)
            {
                Vector2 temp = point1;

                point1 = point2;
                point2 = temp;
            }

            Vector2 differenceVect = new Vector2(point2.X - point1.X, (float)(Math.Abs(point2.Y - point1.Y)));

            float error = 0.0f;
            int yIncriment;
            int tempY = (int)point1.Y;

            // If slope is positive the yIncriment is positive 1 and of the slope is negative the yIncriment is -1
            if (point1.Y < point2.Y)
            {
                yIncriment = 1;
            }
            else //(point1.Y > point2.Y)
            {
                yIncriment = -1;
            }

            // Loops through every x value from point1 to point2
            for (int currentX = (int)point1.X; currentX <= point2.X; currentX++)
            {
            //    if (isLineRightToLeft)
            //    {
            //        Vector2 temp = point1;

            //        point1 = point2;
            //        point2 = temp;
            //    }

                if (isSlopeSteep || isLineRightToLeft)
                {
                    pointsOnRay.Add(new Vector2(tempY, currentX));

                    //temp
                //    if (isLineRightToLeft)
                //    {
                //        pointsOnRay[pointsOnRay.Count - 1] = SwapVectorCoordinates(pointsOnRay[pointsOnRay.Count - 1]);
                //    }
                }
                else
                {
                    pointsOnRay.Add(new Vector2(currentX, tempY));
                }



                error += differenceVect.Y;

                if (error * 2.0f >= differenceVect.X)
                {
                    tempY += yIncriment;
                    error -= differenceVect.X;
                }

            }

            return pointsOnRay.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsOnRay"></param>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public Vector2 FindIntersection(Vector2[] pointsOnRay, Tile[,] tiles)
        {
            int tileX;
            int tileY;

            // for loop that goes through all the points until it finds a point that is collidable.
            // It then returns the vector of that point

            for (int i = 0; i < pointsOnRay.Length; i++)
            {

                tileX = ((int)(pointsOnRay[i].X / TileManager.TILE_SIDE_LENGTH));
                tileY = ((int)(pointsOnRay[i].Y / TileManager.TILE_SIDE_LENGTH));

                if ((pointsOnRay[i].X == 0) || (pointsOnRay[i].Y == 0) || (pointsOnRay[i].X >= TileManager.TILE_SIDE_LENGTH * TileManager.TILE_X) || (pointsOnRay[i].Y >= TileManager.TILE_SIDE_LENGTH * TileManager.TILE_Y))
                {
                    return pointsOnRay[i];
                }

                if ((tileX >= tiles.GetLength(0)) || (tileY >= tiles.GetLength(1)) || (tileX < 0) || (tileY < 0))
                {
                    return pointsOnRay[i];
                }
                else if (tiles[tileX, tileY].IsCollidible)
                {
                    return pointsOnRay[i];
                }
            }
            return DestinationPos;
        }

        /// <summary>
        /// Swaps the X and Y coordinates of a Vector
        /// </summary>
        /// <param name="point"> Passes through the Vector2 that needs to be modified </param>
        /// <returns> Returns the new vector with the X and Y values switched </returns>
        public Vector2 SwapVectorCoordinates(Vector2 point)
        {
            return new Vector2(point.Y, point.X);
        }
    }
}
