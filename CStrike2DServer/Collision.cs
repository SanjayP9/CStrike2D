// Author: Shawn Verma
// File Name: Collision.cs
// Project Name: Global Offensive
// Creation Date: Dec 31st, 2015
// Modified Date: Jan 20th, 2016
// Description: Handles all collision detections including: circle to circle, line to circle, circle to rectangle, 
//              and line to non-aa rectangle.
using Microsoft.Xna.Framework;
using System;

namespace CStrike2D
{
    public static class Collision
    {
        /// <summary>
        /// Checks the collision of two players
        /// </summary>
        /// <param name="player1">centre of first player</param>
        /// <param name="player2">centre of second player</param>
        /// <param name="playerRadius">the radius of a player</param>
        /// <returns>true or false based on if the players are colliding</returns>
        public static bool PlayerToPlayer(Vector2 player1, Vector2 player2, float playerRadius)
        {
            // if the distance between the players is less than on equal to the player radius,
            // there is a collision. return true;
            return Vector2.Distance(player1, player2) <= playerRadius;
        }

        /// <summary>
        /// Checks the intersection of a circle and a rectangle
        /// </summary>
        /// <param name="player">the player centre</param>
        /// <param name="rect">the rectangle checking collision with</param>
        /// <param name="radius">the radius of a player</param>
        /// <returns>if the player intersects the rectangle</returns>
        public static bool PlayerToRectangle(Vector2 player, Rectangle rect, float radius)
        {
            // Sets the rectangle x and y coordinates to be the centre of the rectangle
            rect.X += rect.Width / 2;
            rect.Y += rect.Height / 2;

            // Finds the distance between the centre of the circle and the centre of the rectangle
            Vector2 circleDistance = new Vector2(Math.Abs(player.X - rect.X), Math.Abs(player.Y - rect.Y));

            // If the circle is far enough away from the rectangle return false
            if (circleDistance.X > (rect.Width * 0.5f + radius) || circleDistance.Y > (rect.Height * 0.5f + radius))
            {
                return false;
            }

            // If the circle is close enough for a garanteed intersection return true
            if (circleDistance.X <= (rect.Width * 0.5f) || circleDistance.Y <= (rect.Height * 0.5f))
            {
                return true;
            }

            // Check to see if the circle intersects to corner of the rectangle by checking to see if the distance
            // from the centre of the circle to the corner is less than or equal to the radius
            float cornerDistanceSquared = (circleDistance.X - rect.Width * 0.5f) * (circleDistance.X - rect.Width * 0.5f) +
                                      (circleDistance.Y - rect.Height * 0.5f) * (circleDistance.Y - rect.Height * 0.5f);
            return (cornerDistanceSquared <= (radius * radius));
        }

        /// <summary>
        /// Checks if the bullet emitted from a player at a specified angle intersects an enemy player(checks line to circle)
        /// if that is true check non-axis aligned rectangle
        /// </summary>
        /// <param name="shootingPlayer">The shooting player centre</param>
        /// <param name="enemyPlayer">The enemy player centre</param>
        /// <param name="shotAngle">the shot angle</param>
        /// <param name="playerRadius"> the player radius</param>
        /// <returns>if the bullet collid</returns>
        public static bool BulletToPlayer(Vector2 shootingPlayer, Vector2 enemyPlayer, float shotAngle, float playerRadius)
        {
            // Find the linear equation of the shot bullet
            float mPlayer = (float)Math.Tan(shotAngle);
            float bPlayer = shootingPlayer.Y - mPlayer * shootingPlayer.X;

            // Find the enemy b value where slope is the negative of the player's using the enemy's centre
            float bEnemy = enemyPlayer.Y + mPlayer * enemyPlayer.X;

            // The point of intersection between the negative slope line of the enemy
            // and the bullet line
            float poiX = (bEnemy - bPlayer) / (2 * mPlayer);
            float poiY = mPlayer * poiX + bPlayer;

            // Returns if the poi is less than or equal to the radius
            return (Vector2.Distance(new Vector2(poiX, poiY), enemyPlayer) <= playerRadius);
        }

        /// <summary>
        /// Checks non-AA collision using matrices
        /// </summary>
        /// <param name="shootingPlayer">the centre of the shooting player</param>
        /// <param name="shotAngle">the shot angle</param>
        /// <param name="enemyPart">the enemy part that you are checking</param>
        /// <param name="enemyRotation">the enemy roatation</param>
        /// <returns></returns>
        public static bool NonAACollision(Vector2 shootingPlayer, float shotAngle, Rectangle enemyPart, float enemyRotation)
        {
            // Find centre and distance from centre to top left corner
            Vector2 centre = new Vector2(enemyPart.X + enemyPart.Width * 0.5f, enemyPart.Y - enemyPart.Height * 0.5f);
            float distance = Vector2.Distance(new Vector2(enemyPart.X, enemyPart.Y), centre);

            //// Find the linear equation of the shot bullet
            float mPlayer = (float)Math.Tan(shotAngle);
            float bPlayer = shootingPlayer.Y - mPlayer * shootingPlayer.X;

            // Holds the points of the rectangle before being rotated
            float x = enemyPart.X - centre.X;
            float y = enemyPart.Y - centre.Y;

            /////////////////////////////////////////////
            // FIND ROTATED COORDINATES OF EACH CORNER //
            /////////////////////////////////////////////
            Vector2 topLeft = new Vector2((float)(x * Math.Cos(enemyRotation) - y * Math.Sin(enemyRotation)) + centre.X,
                                          (float)(y * Math.Cos(enemyRotation) + x * Math.Sin(enemyRotation)) + centre.Y);

            x = enemyPart.X + enemyPart.Width - centre.X;
            y = enemyPart.Y - centre.Y;
            Vector2 topRight = new Vector2((float)(x * Math.Cos(enemyRotation) - y * Math.Sin(enemyRotation)) + centre.X,
                                           (float)(y * Math.Cos(enemyRotation) + x * Math.Sin(enemyRotation)) + centre.Y);

            x = enemyPart.X - centre.X;
            y = enemyPart.Y + enemyPart.Height - centre.Y;
            Vector2 bottomLeft = new Vector2((float)(x * Math.Cos(enemyRotation) - y * Math.Sin(enemyRotation)) + centre.X,
                                             (float)(y * Math.Cos(enemyRotation) + x * Math.Sin(enemyRotation)) + centre.Y);

            x = enemyPart.X + enemyPart.Width - centre.X;
            y = enemyPart.Y + enemyPart.Height - centre.Y;
            Vector2 bottomRight = new Vector2((float)(x * Math.Cos(enemyRotation) - y * Math.Sin(enemyRotation)) + centre.X,
                                              (float)(y * Math.Cos(enemyRotation) + x * Math.Sin(enemyRotation)) + centre.Y);

            // Creates slope from top left to bottom right corner 
            // Finds a P.O.I. with the shot angle and the line created
            float m1 = ((topLeft.Y - bottomRight.Y) / (topLeft.X - bottomRight.X));
            float b1 = centre.Y - m1 * centre.X;
            float poiX1 = (b1 - bPlayer) / (mPlayer - m1);
            float poiY1 = m1 * poiX1 + b1;

            // Creates slope from top right to bottom left corner 
            // Finds a P.O.I. with the shot angle and the line created
            float m2 = ((topRight.Y - bottomLeft.Y) / (topRight.X - bottomLeft.X));
            float b2 = centre.Y - m2 * centre.X;
            float poiX2 = (b2 - bPlayer) / (mPlayer - m2);
            float poiY2 = m2 * poiX2 + b2;

            // If a P.O.I is within the rectangle return true
            if(Vector2.Distance(centre, new Vector2(poiX1,poiY1)) <= distance || 
               Vector2.Distance(centre, new Vector2(poiX2,poiY2)) <= distance)
            {
                return true;
            }

            // If a P.O.I is not within the rectangle return false
            return false;
        }
    }
}
