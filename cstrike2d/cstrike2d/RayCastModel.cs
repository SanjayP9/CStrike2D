using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class RayCastModel
    {
        //Newest Version
        public RayCastView View { get; private set; }

        public Vector2 CollisionPos { get; private set; }
        public Vector2 EmitPos { get; private set; }
        public Vector2 DirectionVect { get; private set; }
        public float RayLength { get; private set; }
        public float Angle { get; private set; }

        public Color RayColor { get; private set; }

        public RayCastResult RayCastLine { get; private set; }

        public RayCastModel()
        {
            View = new RayCastView(this);
            RayCastLine = new RayCastResult();
        }

        public void Update(Vector2 emitPos, Vector2 directionVect, float rayLineLength, Tile[,] tiles, float angle)
        {
            RayCastLine = RayCastMethod(emitPos, directionVect, rayLineLength, tiles, angle);
            CollisionPos = RayCastLine.CollisionPos;

            if (GetRayLength() > 400f)
            {
                RayColor = Color.Green;
            }
            else if (GetRayLength() > 250f)
            {
                RayColor = Color.Yellow;
            }
            else // if (GetRayLength<100f)
            {
                RayColor = Color.Red;
            }
        }

        public RayCastResult RayCastMethod(Vector2 emitPos, Vector2 directionVect, float rayLineLength, Tile[,] tiles, float angle)
        {

            this.EmitPos = emitPos;
            this.DirectionVect = directionVect;
            this.RayLength = rayLineLength;
            this.Angle = angle;


            RayCastResult castResult = new RayCastResult();

            if (rayLineLength == 0f)
            {
                castResult.CollisionPos = emitPos;
                castResult.IsColliding = (IsVectorAccessible(emitPos, tiles));

                return castResult;
            }

            // 
            directionVect.Normalize();

            Vector2[] pointsOnRay = GetPointsOnRay(emitPos, emitPos + (directionVect * rayLineLength));

            if (pointsOnRay.Length > 0)
            {
                int index = 0;

                if (pointsOnRay[0] != emitPos)
                {
                    index = pointsOnRay.Length - 1;
                }

                while (true)
                {
                    Vector2 tempPoint = pointsOnRay[index];

                    if (!(IsVectorAccessible(tempPoint, tiles)))
                    {
                        castResult.CollisionPos = tempPoint;
                        castResult.IsColliding = true;
                        break;
                    }
                    if (pointsOnRay[0] != emitPos)
                    {
                        index--;

                        if (index < 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        index++;

                        if (index >= pointsOnRay.Length)
                        {
                            break;
                        }
                    }
                }

            }
            return castResult;
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


        public bool IsVectorAccessible(Vector2 point, Tile[,] tiles)
        {
            // Gets current tile based on the coordinates of the point. Math.Floor was used to
            // get the tile number because tileX and tileY cannot be decimals.
            int tileX = (int)Math.Floor(point.X / TileManager.TILE_SIDE_LENGTH);
            int tileY = (int)Math.Floor(point.Y / TileManager.TILE_SIDE_LENGTH);

            // If the tile is ever outside of the 2D tile array then the point will be assumed inaccessable and will be treated
            // like a solid tile
            if ((tileX < 0) || (tileY < 0) || (tileX >= TileManager.TILE_X) || (tileY >= TileManager.TILE_Y))
            {
                return false;
            }

            // Using tileX and tileY checks if the tile stored in the 2D tile array at that location is collidable
            // If point is collidable it cannot be accessible
            return !(tiles[tileX, tileY].IsSolid);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        //  public Vector2[] GetPointsOnRayDDA(Vector2 point1, Vector2 point2, Vector2 directionVect)
        //   {
        //        Vector2[] pointsOnRay;



        //        return pointsOnRay;
        //     }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public Vector2[] GetPointsOnRay(Vector2 point1, Vector2 point2)
        {


            bool isLineSteep = (Math.Abs(point2.Y - point1.Y) >= Math.Abs(point2.X - point1.X));

            if (isLineSteep)
            {
                point1 = SwapVectorCoordinates(point1);
                point2 = SwapVectorCoordinates(point2);
            }

            if (point1.X > point2.X)
            {
                Vector2 temp = point1;
                point1 = point2;
                point2 = temp;
            }

            Vector2 differenceVect = new Vector2(point2.X - point1.X, Math.Abs(point2.Y - point1.Y));
            int error = 0;
            float yChange;
            float currentY = point1.Y;

            if (point1.Y > point2.Y)
            {
                yChange = -1;
            }
            else
            {
                yChange = 1;
            }

            Vector2[] pointsOnRay = new Vector2[((int)Math.Ceiling(Math.Abs(point2.X - point1.X))) + 1];

            for (int currentX = 0; currentX <= (Math.Abs(point2.X - point1.X)); currentX++)
            {
                if (isLineSteep == true)
                {
                    pointsOnRay[currentX] = new Vector2(currentY, currentX + point1.X);
                }
                else
                {
                    pointsOnRay[currentX] = new Vector2(currentX + point1.X, currentY);
                }

                error += (int)differenceVect.Y;

                if ((error * 2) >= differenceVect.X)
                {
                    currentY += yChange;
                    error -= (int)differenceVect.X;
                }
            }

            return pointsOnRay;
        }

        public float GetRayLength()
        {
            return (RayCastLine.CollisionPos - EmitPos).Length();
        }

    }

    public class RayCastResult
    {
        public bool IsColliding { get; set; }
        public Vector2 CollisionPos { get; set; }
    }
}
