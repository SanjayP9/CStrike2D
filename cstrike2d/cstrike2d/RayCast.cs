using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CStrike2D
{
    class RayCast
    {

        // Used to store the point of interseciton after the raycast is complete
        public Vector2 CollisionPos { get; private set; }

        // Stores the Vector2 point where the ray is emitted from
        private Vector2 emitPos;

        // Stores direction vector
        private Vector2 directionVect;

        // records the length of the ray. From emitPos to CollisionPos
        private  float rayLength;

        // Stores the angle of the ray
        private  float angle;

        // Stores an instance of raycast ressult which is used to
        // store the isColliding bool and the collsion point
        public RayCastResult RayCastLine { get; private set; }

        // Controls color of the ray line
        public Color RayColor;

        /// <summary>
        /// Creates an instance of RayCastModel
        /// </summary>
        public RayCast()
        {
            RayCastLine = new RayCastResult();
        }

        /// <summary>
        /// Runs update logic for a raycast
        /// </summary>
        /// <param name="emitPos"> Passes through ray emit vector </param>
        /// <param name="directionVect"> passes throguh direction vector for the ray </param>
        /// <param name="rayLineLength"> Passes through the max length of the ray </param>
        /// <param name="tiles"> Passes through the tiles on the map </param>
        /// <param name="angle"> Passes through the angle of the ray </param>
        public void Update(Vector2 emitPos, Vector2 directionVect, float rayLineLength, Tile[,] tiles, float angle)
        {
            // Runs raycast method for and stores the returned result in RayCastLine
            RayCastLine = RayCastMethod(emitPos, directionVect, rayLineLength, tiles, angle);
            
            //
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

        /// <summary>
        /// This method returns the the collision point and a bool stating whether a collision has been found
        /// </summary>
        /// <param name="emitPos"> Passes through the ray emitter vector </param>
        /// <param name="directionVect"> specifies the direction vector of the ray </param>
        /// <param name="rayLineLength"> Passes through the max ray length </param>
        /// <param name="tiles"> Passes through the map tiles </param>
        /// <param name="angle"> Passes through the angle of the ray </param>
        /// <returns></returns>
        public RayCastResult RayCastMethod(Vector2 emitPos, Vector2 directionVect, float rayLineLength, Tile[,] tiles, float angle)
        {
            // Sets global variables to variables
            this.emitPos = emitPos;
            this.directionVect = directionVect;
            this.rayLength = rayLineLength;
            this.angle = angle;


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


        /// <summary>
        /// Given a Vector2 point and the tiles that are on the map it will return whether or not the Vector2 is inside or on a wall
        /// </summary>
        /// <param name="point"></param>
        /// <param name="tiles"></param>
        /// <returns></returns>
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
        /// Gets all of the points in the ray and stores it in a Vector2 array. Using Bersenhams Line Algorithm
        /// </summary>
        /// <param name="point1"> Starting Vector of ray </param>
        /// <param name="point2"> Ending Vector of ray </param>
        /// <returns> Returns array of Vector2 that are on teh ray </returns>
        public Vector2[] GetPointsOnRay(Vector2 point1, Vector2 point2)
        {
            // True if the angle of the line is more than 45 degrees
            bool isLineSteep = (Math.Abs(point2.Y - point1.Y) >= Math.Abs(point2.X - point1.X));

            // If the bool is true it swaps the x and y of the 
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRayLength()
        {
            return (RayCastLine.CollisionPos - emitPos).Length();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pixelTexture"></param>
        /// <param name="circleTexture"></param>
        public void Draw(SpriteBatch sb, Texture2D pixelTexture, Texture2D circleTexture)
        {
            // Draws the ray with a 1x1 texture with the given angle as the rotation
            sb.Draw(pixelTexture,
                    new Rectangle((int)emitPos.X, (int)emitPos.Y, (int)GetRayLength(), 2),
                    null,
                    RayColor,
                    angle,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
        }

    }

    /// <summary>
    /// Stores the Raycasting result. Used to return the result with a bool and a Vector2
    /// </summary>
    public class RayCastResult
    {
        // Public properties that are used to store and return the Vector2 and bool for collision
        public bool IsColliding { get; set; }
        public Vector2 CollisionPos { get; set; }
    }
}
