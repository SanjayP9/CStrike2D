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
            Vector2 fixOffset = new Vector2(playerRadius, playerRadius);
            shootingPlayer += fixOffset;
            enemyPlayer += fixOffset;

            float mPlayer = (float)Math.Tan(shotAngle);
            float bPlayer = shootingPlayer.Y - mPlayer * shootingPlayer.X;
            float bEnemy = enemyPlayer.Y + mPlayer * shootingPlayer.X;

            float poiX = (bEnemy - bPlayer) / (2 * mPlayer);
            float poiY = mPlayer* poiX + bPlayer;

            Vector2 poi = new Vector2(poiX, poiY);

            return Vector2.Distance(poi, enemyPlayer) <= playerRadius;
        }
    }
}
