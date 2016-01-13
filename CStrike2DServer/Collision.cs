using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
            float mPlayer = (float)Math.Tan(shotAngle);
            
            float bPlayer = shootingPlayer.Y - mPlayer * shootingPlayer.X;
            float bEnemy = enemyPlayer.Y + mPlayer * enemyPlayer.X;

            float poiX = (bEnemy - bPlayer) / (2 * mPlayer);
            float poiY = mPlayer * poiX + bPlayer;

            Vector2 poi = new Vector2(poiX, poiY);
            //float poiA = (float)(Math.Atan((double)((poiY - shootingPlayer.Y) / (poiX - shootingPlayer.X))));

            //if (poiA > Math.PI)
            //{
            //    poiA = (float)Math.PI * 2f + poiA;
            //}
            if (shotAngle < Math.PI/2 && poiX - shootingPlayer.X < 0)
            {
                return false;
            }
            if (shotAngle >= Math.PI)
            {
                shotAngle = (float)Math.PI * 2f + shotAngle;
            }

            if(shotAngle > Math.PI && poiY - shootingPlayer.Y > 0 || shotAngle < Math.PI && poiY - shootingPlayer.Y< 0)
            {
                return false;
            }

            return Vector2.Distance(poi, enemyPlayer) <= playerRadius;
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

        public static bool BtP(Vector2 shooter, Vector2 target, float angle, float radius)
        {
            Vector2 delta = target - shooter;
            float tarShoAngle = (float)Math.Atan2(delta.Y, delta.X);
            float deltaAngle = angle - tarShoAngle;
            float tangentAngle = (float)((Math.PI*0.5f) - deltaAngle);
            //Math.Atan2()
            return false;
        }
    }
}
