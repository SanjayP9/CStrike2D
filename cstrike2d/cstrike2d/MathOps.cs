// Author: Mark Voong
// File Name: MathOps.cs
// Project: cstrike2d
// Creation Date: Oct 9th, 2015
// Modified Date: Oct 9th, 2015
// Description: Holds various useful functions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace cstrike2d
{
    static class MathOps
    {
        private const float QUADRANT_ONE = (float)Math.PI/2f;
        private const float QUADRANT_TWO = (float) Math.PI;
        private const float QUADRANT_THREE = (float) ((3f*Math.PI)/2f);
        private const float QUADRANT_FOUR = (float) (2 * Math.PI);

        public const float TWO_PI_RAD = (float)(2*Math.PI);
        /// <summary>
        /// Returns the delta between two vectors
        /// </summary>
        /// <param name="final"></param>
        /// <param name="initial"></param>
        /// <returns></returns>
        public static Vector2 Delta(Vector2 final, Vector2 initial)
        {
            return final - initial;
        }

        /// <summary>
        /// Returns the angle of two vectors
        /// </summary>
        /// <param name="final"></param>
        /// <param name="initial"></param>
        /// <returns></returns>
        public static float Angle(Vector2 final, Vector2 initial)
        {
            Vector2 delta = final - initial;

            return (float) Math.Round(Math.Atan2(delta.Y, delta.X), 2);
        }

        public static float ToDegrees(float angle)
        {
            angle = (float)((angle*180/Math.PI) + 360)%360;

            return (float)Math.Round(angle, 2);
        }

        /// <summary>
        /// Converts the Inverse Tangent (-Pi - +Pi) to rads where angle cannot be
        /// less than zero
        /// </summary>
        /// <param name="arctanAngle"></param>
        /// <returns></returns>
        public static float RealRadians(float arctanAngle)
        {
            if (arctanAngle < 0)
            {
                return (float)Math.Round(arctanAngle + TWO_PI_RAD, 2);
            }
            return arctanAngle;
        }

        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Returns the quadrant of an angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static int ReturnQuadrant(float angle)
        {
            // Starting from the right side, in a closewise direction
            // 1 = Bottom-Right (PI / 2)
            // 2 = Bottom-Left (PI)
            // 3 = Top-Left (3 * PI / 2)
            // 4 = Top-Right (2 PI)
            // 0 = No Angle (should not happen)

            if (angle <= QUADRANT_ONE)
            {
                return 1;
            }

            if (angle <= QUADRANT_TWO)
            {
                return 2;
            }

            if (angle <= QUADRANT_THREE)
            {
                return 3;
            }

            if (angle <= QUADRANT_FOUR)
            {
                return 4;
            }

            return 0;
        }
    }
}
