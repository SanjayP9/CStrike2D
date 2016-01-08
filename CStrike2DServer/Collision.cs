using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    class Collision
    {
        public bool PlayerToPlayer(Vector2 player1, Vector2 player2, float playerRadius)
        {
            return Vector2.Distance(player1, player2) <= playerRadius;
        }
        public bool BulletToPlayer(Vector2 playerOrigin, float angle, float radius)
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
        public bool BulletToP(Vector2 playerOrigin, float playerAngle)
        {
            int playerH = 32;
            int playerHd2 = 16;

            // Solve for P.O.I
            // Create line y = ax + b
            float a = (float)Math.Tan(playerAngle);
            float b = playerOrigin.Y + (a * (playerOrigin.X)); //maybe minus
            return false;
        }
    }
}
