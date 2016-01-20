// Author: Rob Stephens
// File Name: Triangle.cs
// Creation Date: Added to project Jan 16th, 2016
// Description: Used in triangulation of a polygon
// - Edits were made to this class, specifically converting
// the FloatF type into Vector2
using Microsoft.Xna.Framework;

namespace PolygonTriangulation
{
    /// <summary>
    /// A triangle consisting of 3 sides
    /// </summary>
    public class Triangle : Polygon
    {
        public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            Points = new [] { p0, p1, p2 };
        }
    }
}
