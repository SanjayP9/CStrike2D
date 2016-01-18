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
        public float ZoomFactor { get; private set; }

        public Camera2D()
        {
            Position = Vector2.Zero;
            ZoomFactor = 1.2f;
        }

        public Matrix GetTransform(GraphicsDevice graphics)
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))*
                     Matrix.CreateRotationX(0)*
                     Matrix.CreateScale(new Vector3(ZoomFactor, ZoomFactor, 0))*
                     Matrix.CreateTranslation(new Vector3(graphics.Viewport.Width*0.5f, graphics.Viewport.Height*0.5f, 0));

            return Transform;
        }

        public void IncreaseZoom()
        {
            ZoomFactor += 0.1f;

            ZoomFactor = MathHelper.Clamp(ZoomFactor, 0.1f, 2f);
        }

        public void DecreaseZoom()
        {
            ZoomFactor -= 0.1f;

            ZoomFactor = MathHelper.Clamp(ZoomFactor, 0.1f, 2f);
        }

        public void ResetZoom()
        {
            ZoomFactor = 1.2f;
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
