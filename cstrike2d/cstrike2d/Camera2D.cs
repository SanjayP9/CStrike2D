// Author: Mark Voong
// File Name: Camera2D.cs
// Project Name: Global Offensive
// Creation Date: Dec 21st, 2015
// Modified Date: Jan 18th, 2016
// Description: Handles logic for the camera
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Camera2D
    {
        /// <summary>
        /// Matrix used to move everything relative to the camera
        /// </summary>
        public Matrix Transform { get; private set; }

        /// <summary>
        /// The position of the camera
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The zoom factor of the camera
        /// </summary>
        public float ZoomFactor { get; private set; }

        /// <summary>
        /// Initializes the camera
        /// </summary>
        public Camera2D()
        {
            Position = Vector2.Zero;
            ZoomFactor = 1.2f;
        }

        /// <summary>
        /// Returns the matrix of the camera
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        public Matrix GetTransform(GraphicsDevice graphics)
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))*
                     Matrix.CreateRotationX(0)*
                     Matrix.CreateScale(new Vector3(ZoomFactor, ZoomFactor, 0))*
                     Matrix.CreateTranslation(new Vector3(graphics.Viewport.Width*0.5f, graphics.Viewport.Height*0.5f, 0));

            return Transform;
        }

        /// <summary>
        /// Increases zoom
        /// </summary>
        public void IncreaseZoom()
        {
            if (ZoomFactor <= 1.0f)
            {
                ZoomFactor += 0.1f;
            }
            else
            {
                ZoomFactor += 0.05f;
            }

            ZoomFactor = MathHelper.Clamp(ZoomFactor, 0.05f, 2f);
        }

        /// <summary>
        /// Decreases zoom
        /// </summary>
        public void DecreaseZoom()
        {
            if (ZoomFactor >= 1.0f)
            {
                ZoomFactor -= 0.1f;
            }
            else
            {
                ZoomFactor -= 0.05f;
            }

            ZoomFactor = MathHelper.Clamp(ZoomFactor, 0.05f, 2f);
        }

        /// <summary>
        /// Resets the zoom 
        /// </summary>
        public void ResetZoom()
        {
            ZoomFactor = 1.2f;
        }
    }
}
