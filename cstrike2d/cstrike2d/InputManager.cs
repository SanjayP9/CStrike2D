// Author: Mark Voong
// File Name: InputManager.cs
// Project: cstrike2d
// Creation Date: Sept 10th, 2015
// Modified Date: Sept 20th, 2015
// Description: Handles all input from the user

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    class InputManager
    {
        MouseState prevMouseState, mouseState;
        KeyboardState prevKeyboardState, keyboardState;
        private float prevMouseScroll;

        /// <summary>
        /// Returns the mouse position as a Vector2
        /// </summary>
        public Vector2 MousePosition { get; private set; }

        /// <summary>
        /// Updates a fresh set of inputs from the user to compare with
        /// </summary>
        public void Tick()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            MousePosition = new Vector2(mouseState.X, mouseState.Y);
        }

        /// <summary>
        /// Saves the current state of inputs to be compared with the next update
        /// </summary>
        public void Tock()
        {
            prevKeyboardState = keyboardState;
            prevMouseState = mouseState;
            prevMouseScroll = mouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Checks if a key was tapped
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Tapped(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && 
                prevKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Checks if a key is being held
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Held(Keys key)
        {
            return (keyboardState.IsKeyDown(key) &&
                prevKeyboardState.IsKeyDown(key));
        }

        /// <summary>
        /// Checks if the user scrolled up
        /// </summary>
        /// <returns></returns>
        public bool ScrollUp()
        {
            return (mouseState.ScrollWheelValue > prevMouseScroll);
        }

        /// <summary>
        /// Checks if the user scrolled down
        /// </summary>
        /// <returns></returns>
        public bool ScrollDown()
        {
            return (mouseState.ScrollWheelValue < prevMouseScroll);
        }

        public float MouseRotation(Camera2D origin)
        {
            Vector2 delta = MousePosition - Vector2.Transform(origin.Position, origin.Transform);

            return (float)Math.Round(Math.Atan2(delta.Y, delta.X), 2);
        }

        public Vector2 Delta(Camera2D origin)
        {
            Vector2 delta = Vector2.Transform(MousePosition, origin.Transform) - origin.Position;
            return delta;
        }

        public Vector2 MouseLocal(Matrix transform)
        {
            return new Vector2(Vector2.Transform(MousePosition, transform).X, Vector2.Transform(MousePosition, transform).Y);
        }

        public Vector2 ScreenToWorld(Vector2 vector, Camera2D origin, Vector2 center)
        {
            return (vector + origin.Position - center);
        }

        public int GetRow(Camera2D origin, Vector2 center)
        {
            return (int)(ScreenToWorld(MousePosition, origin, center).Y/64f);
        }

        public int GetColumn(Camera2D origin, Vector2 center)
        {
            return (int)(ScreenToWorld(MousePosition, origin, center).X/64f);
        }

        public Vector2 WorldToScreen(Vector2 vector, Camera2D origin)
        {
            return (vector - origin.Position);
        }
    }
}
