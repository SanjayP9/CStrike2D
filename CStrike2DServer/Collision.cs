using CStrike2DServer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    public static class Collision
    {

        public static bool PlayerToPlayer(Vector2 player1, Vector2 player2, float playerRadius)
        {
            return Vector2.Distance(player1, player2) <= playerRadius;
        }
        public static bool BulletToPerson(Vector2 shootingPlayer, Vector2 enemyPlayer, float shotAngle, float playerRadius)
        {
            float enemyToPlayerA = (float)(Math.Atan2(Math.Abs(enemyPlayer.Y - shootingPlayer.Y), Math.Abs(enemyPlayer.X - shootingPlayer.X)));
            if (shotAngle < 0f)
            {
                shotAngle = (float)Math.PI * 2f + shotAngle;
            }
            if (enemyToPlayerA < 0f)
            {
                enemyToPlayerA = (float)Math.PI * 2f + shotAngle;
            }
            if (shotAngle + Math.PI/12f < enemyToPlayerA || shotAngle - Math.PI/12f > enemyToPlayerA)
            {
                return false;
            }
            
            float mPlayer = (float)Math.Tan(shotAngle);
            float bPlayer = shootingPlayer.Y - mPlayer * shootingPlayer.X;
            float bEnemy = enemyPlayer.Y + mPlayer * enemyPlayer.X;

            float poiX = (bEnemy - bPlayer) / (2 * mPlayer);
            float poiY = mPlayer * poiX + bPlayer;

            Vector2 poi = new Vector2(poiX, poiY);
            

            return Vector2.Distance(poi, enemyPlayer) <= playerRadius;
        }

        public static bool LineRectangle(Rectangle enemyPlayer, float shotAngle, float mPlayer, float bPlayer)
        {
            // Find centre and distance from centre to top left corner
            Vector2 centre = new Vector2(enemyPlayer.X + enemyPlayer.Width * 0.5f, enemyPlayer.Y - enemyPlayer.Height * 0.5f);
            float distance = Vector2.Distance(new Vector2(enemyPlayer.X, enemyPlayer.Y), centre);

            // Holds the points of the rectangle before being rotated
            float x = enemyPlayer.X - centre.X;
            float y = enemyPlayer.Y - centre.Y;

            /////////////////////////////////////////////
            // FIND ROTATED COORDINATES OF EACH CORNER //
            /////////////////////////////////////////////
            Vector2 topLeft = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                          (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

            x = enemyPlayer.X + enemyPlayer.Width - centre.Y;
            y = enemyPlayer.Y - centre.Y;
            Vector2 topRight = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                           (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

            x = enemyPlayer.X - centre.Y;
            y = enemyPlayer.Y + enemyPlayer.Height - centre.Y;
            Vector2 bottomLeft = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                             (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

            x = enemyPlayer.X + enemyPlayer.Width - centre.Y;
            y = enemyPlayer.Y + enemyPlayer.Height - centre.Y;
            Vector2 bottomRight = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                              (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

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
            float poiY2 = m2 * poiX1 + b2;

            // If a P.O.I is within the rectangle return true
            if(Vector2.Distance(centre, new Vector2(poiX1,poiY1)) <= distance || 
               Vector2.Distance(centre, new Vector2(poiX2,poiY2)) <= distance)
            {
                return true;
            }

            // If a P.O.I is not within the rectangle return false
            return false;
        }

        public static bool CircleToRectangle(Vector2 circle, Rectangle rect, float radius)
        {
            // Sets the rectangle x and y coordinates to be the centre of the rectangle
            rect.X += rect.Width / 2;
            rect.Y += rect.Height / 2;

            // Finds the distance between the centre of the circle and the centre of the rectangle
            Vector2 circleDistance = new Vector2(Math.Abs(circle.X - rect.X), Math.Abs(circle.Y - rect.Y));

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
            float cornerDistance_sq = (circleDistance.X - rect.Width * 0.5f) * (circleDistance.X - rect.Width * 0.5f) +
                                      (circleDistance.Y - rect.Height * 0.5f) * (circleDistance.Y - rect.Height * 0.5f);
            return (cornerDistance_sq <= (radius * radius));
        }
    }
}
