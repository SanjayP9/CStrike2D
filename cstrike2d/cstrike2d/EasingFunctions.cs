// Author: Mark Voong
// File Name: EasingFunctions.cs
// Project Name: LightEngine
// Creation Date: Feburary 10th, 2015
// Modified Date: May 18th, 2015
// Description: See Below.
using System;

namespace LightEngine
{
    /// <summary>
    /// Author: Mark Voong
    /// -------------------------------------------------------------------------------------------
    /// Description:  Part of the LightEngine library designed for reusablility and easy
    ///               implementation across multiple applications. This class only contains static
    ///               functions for various types of animation transisitions.
    /// -------------------------------------------------------------------------------------------
    /// Setup:        All functions require the current time in the animation, a starting point,
    ///               the distance between the destination, and the
    ///               animation time.
    /// -------------------------------------------------------------------------------------------
    /// Main Methods: Linear, Quadratic, Cubic, Quartic, Quintic, Sinusoidal, Exponential, Circular
    ///               All functions have an In, Out, and In/Out version.
    /// -------------------------------------------------------------------------------------------
    /// </summary>
    static class EasingFunctions
    {
        public enum AnimationType
        {
            Linear,
            QuadraticIn,
            QuadraticOut,
            QuadraticInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            QuarticIn,
            QuarticOut,
            QuarticInOut,
            QuinticIn,
            QuinticOut,
            QuinticInOut,
            SinIn,
            SinOut,
            SinInOut,
            ExpIn,
            ExpOut,
            ExpInOut,
            CircIn,
            CircOut,
            CircInOut
        }

        /// <summary>
        /// Returns the appropriate function given the
        /// requested animation type
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startingPoint"></param>
        /// <param name="change"></param>
        /// <param name="animationTime"></param>
        /// <param name="animType"></param>
        /// <returns></returns>
        public static double Animate(double time, double startingPoint, double change, double animationTime, AnimationType animType)
        {
            // If the animation is complete
            // Return the destination to avoid overshoot
            if (time > animationTime)
            {
                return startingPoint + change;
            }

            switch (animType)
            {
                case AnimationType.Linear:
                    return Linear(time, startingPoint, change, animationTime);
                case AnimationType.QuadraticIn:
                    return QuadraticIn(time, startingPoint, change, animationTime);
                case AnimationType.QuadraticOut:
                    return QuadraticOut(time, startingPoint, change, animationTime);
                case AnimationType.QuadraticInOut:
                    return QuadraticInOut(time, startingPoint, change, animationTime);
                case AnimationType.CubicIn:
                    return CubicIn(time, startingPoint, change, animationTime);
                case AnimationType.CubicOut:
                    return CubicOut(time, startingPoint, change, animationTime);
                case AnimationType.CubicInOut:
                    return CubicInOut(time, startingPoint, change, animationTime);
                case AnimationType.QuarticIn:
                    return QuarticIn(time, startingPoint, change, animationTime);
                case AnimationType.QuarticOut:
                    return QuarticOut(time, startingPoint, change, animationTime);
                case AnimationType.QuarticInOut:
                    return QuarticInOut(time, startingPoint, change, animationTime);
                case AnimationType.QuinticIn:
                    return QuinticIn(time, startingPoint, change, animationTime);
                case AnimationType.QuinticOut:
                    return QuinticOut(time, startingPoint, change, animationTime);
                case AnimationType.QuinticInOut:
                    return QuinticInOut(time, startingPoint, change, animationTime);
                case AnimationType.SinIn:
                    return SinIn(time, startingPoint, change, animationTime);
                case AnimationType.SinOut:
                    return SinOut(time, startingPoint, change, animationTime);
                case AnimationType.SinInOut:
                    return SinInOut(time, startingPoint, change, animationTime);
                case AnimationType.ExpIn:
                    return ExpIn(time, startingPoint, change, animationTime);
                case AnimationType.ExpOut:
                    return ExpOut(time, startingPoint, change, animationTime);
                case AnimationType.ExpInOut:
                    return ExpInOut(time, startingPoint, change, animationTime);
                case AnimationType.CircIn:
                    return CircIn(time, startingPoint, change, animationTime);
                case AnimationType.CircOut:
                    return CircOut(time, startingPoint, change, animationTime);
                case AnimationType.CircInOut:
                    return CircInOut(time, startingPoint, change, animationTime);
                default:
                    throw new Exception("An Invalid Enum was given");
            }
        }

        #region Linear

        private static double Linear(double t, double b, double c, double d)
        {
            return c * t / d + b;
        }

        #endregion

        #region Quadratic

        private static double QuadraticIn(double t, double b, double c, double d)
        {
            t /= d;
            return c * t * t + b;
        }

        private static double QuadraticOut(double t, double b, double c, double d)
        {
            t /= d;
            return -c * t * (t - 20) + b;
        }

        private static double QuadraticInOut(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1)
            {
                return c / 2 * t * t + b;
            }
            t--;
            return -c / 2 * (t * (t - 2) - 1) + b;
        }

        #endregion

        #region Cubic

        private static double CubicIn(double t, double b, double c, double d)
        {
            t /= d;
            return c * t * t * t + b;
        }

        private static double CubicOut(double t, double b, double c, double d)
        {
            t /= d;
            t--;
            return c * (t * t * t + 1) + b;
        }

        private static double CubicInOut(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1)
            {
                return c / 2 * t * t * t + b;
            }
            t -= 2;
            return c / 2 * (t * t * t + 2) + b;
        }

        #endregion

        #region Quartic

        private static double QuarticIn(double t, double b, double c, double d)
        {
            t /= d;
            return c * t * t * t * t + b;
        }

        private static double QuarticOut(double t, double b, double c, double d)
        {
            t /= d;
            t--;
            return -c * (t * t * t * t - 1) + b;
        }

        private static double QuarticInOut(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t + b;
            t -= 2;
            return -c / 2 * (t * t * t * t - 2) + b;
        }

        #endregion

        #region Quintic

        private static double QuinticIn(double t, double b, double c, double d)
        {
            t /= d;
            return c * t * t * t * t * t + b;
        }

        private static double QuinticOut(double t, double b, double c, double d)
        {
            t /= d;
            t--;
            return c * (t * t * t * t * t + 1) + b;
        }

        private static double QuinticInOut(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t * t * t + 2) + b;
        }

        #endregion

        #region Sinusoidal

        private static double SinIn(double t, double b, double c, double d)
        {
            return (-c * Math.Cos(t / d * (Math.PI / 2)) + c + b);
        }

        private static double SinOut(double t, double b, double c, double d)
        {
            return (c * Math.Sin(t / d * (Math.PI / 2)) + b);
        }

        private static double SinInOut(double t, double b, double c, double d)
        {
            return (-c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b);
        }

        #endregion

        #region Exponential

        private static double ExpIn(double t, double b, double c, double d)
        {
            return (c * Math.Pow(2, 10 * (t / d - 1)) + b);
        }

        private static double ExpOut(double t, double b, double c, double d)
        {
            return (c * (-Math.Pow(2, -10 * t / d) + 1) + b);
        }

        private static double ExpInOut(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1)
            {
                return (c / 2 * Math.Pow(2, 10 * (t - 1)) + b);
            }
            t--;
            return (c / 2 * (-Math.Pow(2, -10 * t) + 2) + b);
        }

        #endregion

        #region Circular

        private static double CircIn(double t, double b, double c, double d)
        {
            t /= d;
            return (-c * (Math.Sqrt(1 - t * t) - 1) + b);
        }

        private static double CircOut(double t, double b, double c, double d)
        {
            t /= d;
            t--;
            return (c * Math.Sqrt(1 - t * t) + b);
        }

        private static double CircInOut(double t, double b, double c, double d)
        {
            t /= d / 2;
            if (t < 1)
            {
                return (-c / 2 * (Math.Sqrt(1 - t * t) - 1) + b);
            }
            t -= 2;
            return (c / 2 * (Math.Sqrt(1 - t * t) + 1) + b);
        }

        #endregion
    }
}
