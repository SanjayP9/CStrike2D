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

        public static bool BulletToPlayer(Vector2 playerOrigin, float angle, float radius)
        {
            // Solve for P.O.I
            // Create line y = ax + b
            float a = (float)Math.Tan(angle);
            float b = playerOrigin.Y + (a * (playerOrigin.X)); //maybe minus

            // Solve for x and y
            float x = (b / (2 * a));
            float y = (b * 0.5f);

            // P.O.I
            Vector2 intersect = new Vector2(x, y);

            // Line-Circle Collision
            float distance = Vector2.Distance(playerOrigin, intersect);

            return (radius <= distance);
        }

        public static bool BulletToPerson(Vector2 shootingPlayer, Vector2 enemyPlayer, float shotAngle, float playerRadius)
        {
            double enemytoPlayerA = Math.Atan((double)((enemyPlayer.Y - shootingPlayer.Y)/(enemyPlayer.X - shootingPlayer.X)));
            if (0 <= shotAngle && shotAngle < Math.PI)
            {
                return false;
                shotAngle = (float)Math.PI * 2f + shotAngle;
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
            // Find centre and distance from centre to top right corner
            Vector2 centre = new Vector2(enemyPlayer.X + enemyPlayer.Width * 0.5f, enemyPlayer.Y - enemyPlayer.Height * 0.5f);
            float distance = Vector2.Distance(new Vector2(enemyPlayer.X, enemyPlayer.Y), centre);

            // Holds the points of the rectangle before being rotated
            float x = enemyPlayer.X;
            float y = enemyPlayer.Y;

            /////////////////////////////////
            // FIND ROTATION OF EACH CORNER//
            /////////////////////////////////
            Vector2 topLeft = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                          (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

            x = enemyPlayer.X + enemyPlayer.Width;
            y = enemyPlayer.Y;
            Vector2 topRight = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                           (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

            x = enemyPlayer.X;
            y = enemyPlayer.Y + enemyPlayer.Height;
            Vector2 bottomLeft = new Vector2((float)(x * Math.Cos(shotAngle) - y * Math.Sin(shotAngle)) + centre.X,
                                             (float)(y * Math.Cos(shotAngle) + x * Math.Sin(shotAngle)) + centre.Y);

            x = enemyPlayer.X + enemyPlayer.Width;
            y = enemyPlayer.Y + enemyPlayer.Height;
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
            Vector2 circleDistance = new Vector2(Math.Abs(circle.X - rect.X), Math.Abs(circle.Y - rect.Y));

            if (circleDistance.X > (rect.Width * 0.5f + radius) || circleDistance.Y > (rect.Height * 0.5f + radius)) 
            {
                return false; 
            }

            if (circleDistance.X <= (rect.Width * 0.5f) || circleDistance.Y <= (rect.Height * 0.5f)) 
            {
                return true; 
            }

            float cornerDistance_sq = (circleDistance.X - rect.Width / 2) * (circleDistance.X - rect.Width / 2) +
                                      (circleDistance.Y - rect.Height / 2) * (circleDistance.Y - rect.Height / 2);

            return (cornerDistance_sq <= (radius * radius));
        }
    }
}
