// Author: Mark Voong
// File Name: CStrikeModel.cs
// Project Name: CStrike2D
// Creation Date: Dec 21st, 2015
// Modified Date: Jan 3rd, 2016
// Description: Handles logic for the camera
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Camera2D
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position;

        public Camera2D()
        {
            Position = Vector2.Zero;
        }

        public Matrix GetTransform(GraphicsDevice graphics)
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))*
                     Matrix.CreateRotationX(0)*
                     Matrix.CreateScale(new Vector3(1, 1, 0))*
                     Matrix.CreateTranslation(new Vector3(graphics.Viewport.Width*0.5f, graphics.Viewport.Height*0.5f, 0));

            return Transform;
        }

        public int Row()
        {
            return (int) (Position.Y/64);
        }

        public int Col()
        {
            return (int) (Position.X/64);
        }
    }
}
