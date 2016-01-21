// Author: Sanjay Paraboo
// File Name: RayCast.cs
// Project Name: Global Offensive
// Creation Date: Dec 16th, 2015
// Modified Date: Jan 19th, 2016
// Description: Casts a ray and checks if the path collides with any solid (Rectangle)
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        private float rayLength;

        // Stores the angle of the ray
        private float angle;

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
            // RayCastLine = new RayCastResult();
        }

        /// <summary>
        /// Runs update logic for a raycast
        /// </summary>
        /// <param name="emitPos"> Passes through ray emit vector </param>
        /// <param name="directionVect"> passes throguh direction vector for the ray </param>
        /// <param name="rayLineLength"> Passes through the max length of the ray </param>
        /// <param name="tiles"> Passes through the tiles on the map </param>
        /// <param name="angle"> Passes through the angle of the ray </param>
        public void Update(Vector2 emitPos, float rayLineLength, Map map, float angle)
        {
            directionVect = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            // Runs raycast method for and stores the returned result in RayCastLine
            RayCastLine = RayCastMethod(emitPos, directionVect, rayLineLength, map, angle);

            // Sets global Collision Pos to the one obtained from RayCastLine
            CollisionPos = RayCastLine.CollisionPos;

            // Sets Ray line oclor based on distance from collision pos
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
        /// <param name="angle"> Passes through the angle of the ray </param>
        /// <returns></returns>
        public RayCastResult RayCastMethod(Vector2 emitPos, Vector2 directionVect, float rayLineLength, Map map, float angle)
        {
            // Sets global variables values to values btained from local variables
            this.emitPos = emitPos;
            this.directionVect = directionVect;
            this.rayLength = rayLineLength;
            this.angle = angle;

            // Creates an instance of rayCastResult
            RayCastResult castResult = new RayCastResult();

            // If the rayLineLength is 0 then t
            if (rayLineLength == 0f)
            {
                castResult.CollisionPos = emitPos;
                castResult.IsColliding = (IsVectorAccessible(emitPos, map));

                return castResult;
            }

            // Normalizes direction vect so that its equivilant to one unit
            directionVect.Normalize();

            // Returns each of the points that are in the ray an stores it in the pointsOnRay array
            Vector2[] pointsOnRay = GetPointsOnRay(emitPos, emitPos + (directionVect * rayLineLength));

            // If theres more than 1 point on the ray run logic below
            if (pointsOnRay.Length > 0)
            {
                int index = 0;

                // If the first element is not at the emit position set the index to the last element in the array
                if (pointsOnRay[0] != emitPos)
                {
                    index = pointsOnRay.Length - 1;
                }

                // Starts while loop
                while (true)
                {
                    // Sets the tempPoint vector tot the point on the index
                    Vector2 tempPoint = pointsOnRay[index];

                    // If the vector at the point above is not accessible then a collision point is found
                    if (!(IsVectorAccessible(tempPoint, map)))
                    {
                        castResult.CollisionPos = tempPoint;
                        castResult.IsColliding = true;
                        break;
                    }

                    // If the first element in pointsOnRAy is not at the emit poisiton yet then decrement current Index
                    if (pointsOnRay[0] != emitPos)
                    {
                        index--;

                        //  If the index gets out of bounds then break form the loop
                        if (index < 0)
                        {
                            break;
                        }
                    }
                    else // If the first index is at the emitter position incriment the index
                    {
                        index++;

                        // Prevents the index from going out of bounds
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
        /// <returns></returns>
        public bool IsVectorAccessible(Vector2 point, Map map)
        {
            // Gets current tile based on the coordinates of the point. Math.Floor was used to
            // get the tile number because tileX and tileY cannot be decimals.
            int tileX = (int)Math.Floor(point.X / Map.TILE_SIZE);
            int tileY = (int)Math.Floor(point.Y / Map.TILE_SIZE);

            // If the tile is ever outside of the 2D tile array then the point will be assumed inaccessable and will be treated
            // like a solid tile
            if ((tileX < 0) || (tileY < 0) || (tileX >= map.MapArea.Width) || (tileY >= map.MapArea.Height) ||
                map.TileMap[tileX, tileY] == null)
            {
                return false;
            }

            // Using tileX and tileY checks if the tile stored in the 2D tile array at that location is collidable
            // If point is collidable it cannot be accessible
            return map.TileMap[tileX, tileY].Property != Tile.SOLID;

        }


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

            // If the bool is true it swaps the x and y of the vectors in order to compensate 
            // for Bersanhams line algorithm since it doesnt function if the angle of the line is more than 45 degrees
            if (isLineSteep)
            {
                point1 = SwapVectorCoordinates(point1);
                point2 = SwapVectorCoordinates(point2);
            }

            // If the line goes right to left it flips the line so that it travels from left to right
            // This is also done to compensate for the algorithm since the algorithm doesnt work with lines from right to left
            if (point1.X > point2.X)
            {
                Vector2 temp = point1;
                point1 = point2;
                point2 = temp;
            }

            // Gets the difference vecotr between the destination and emit vector
            Vector2 differenceVect = new Vector2(point2.X - point1.X, Math.Abs(point2.Y - point1.Y));

            // Error is used to find out how off the algorithms estimate is
            int error = 0;

            // Used to incriment the y value in order to find the next point on the line
            float yChange;
            float currentY = point1.Y;

            // depending on the end behaviors off the line it will find the yChange
            if (point1.Y > point2.Y)
            {
                yChange = -1;
            }
            else
            {
                yChange = 1;
            }

            // Calculates how many points will be on the line and makes the length of the array equal to that value
            Vector2[] pointsOnRay = new Vector2[((int)Math.Ceiling(Math.Abs(point2.X - point1.X))) + 1];


            // For every x value on the line it calculates the next point on the ray and stores the vector in the pointsOnRay array 
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


                // Adds the difference of the y value between the actual line and the one that was calculated in
                //  
                error += (int)differenceVect.Y;

                // Bersenhams line algorithm uses the error calculation below in order to check if based on the last point
                // should it skip the next point or not in order to make the line more accurate
                if ((error * 2) >= differenceVect.X)
                {
                    currentY += yChange;
                    error -= (int)differenceVect.X;
                }
            }

            // Returns the pointsOnRay array
            return pointsOnRay;
        }

        /// <summary>
        /// Used to return the Ray Length
        /// </summary>
        /// <returns></returns>
        public float GetRayLength()
        {
            return (RayCastLine.CollisionPos - emitPos).Length();
        }

        /// <summary>
        /// Draws RayCast line
        /// </summary>
        /// <param name="sb"> Passes through the SpriteBatch instance in order to use the Draw methods </param>
        /// <param name="pixelTexture"> Passes through pixel texture in order to draw the line </param>
        public void Draw(SpriteBatch sb, Texture2D pixelTexture)
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