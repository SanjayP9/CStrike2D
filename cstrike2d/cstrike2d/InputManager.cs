// Author: Mark Voong
// File Name: InputManager.cs
// Project Name: Global Offensive
// Creation Date: Sept 10th, 2015
// Modified Date: Jan 18th, 2016
// Description: Handles all input from the user
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class InputManager
    {
        private MouseState prevMouseState;
        private MouseState mouseState;
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;
        private float prevMouseScroll;
        private bool capslock;

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
        /// Check if a key is up
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyUp(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if a key is down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if a key was tapped
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Tapped(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                   prevKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a key is being held
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Held(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                   prevKeyboardState.IsKeyDown(key);
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

        /// <summary>
        /// Checks if the user clicked the left mouse button
        /// </summary>
        /// <returns></returns>
        public bool LeftClick()
        {
            return prevMouseState.LeftButton == ButtonState.Pressed &&
                   mouseState.LeftButton == ButtonState.Released;
        }

        public bool LeftClickImmediate()
        {
            return prevMouseState.LeftButton == ButtonState.Released &&
                   mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the user clicked the right mouse button
        /// </summary>
        /// <returns></returns>
        public bool RightClick()
        {
            return prevMouseState.RightButton == ButtonState.Pressed &&
                   mouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the user is holding the left mouse button
        /// </summary>
        /// <returns></returns>
        public bool LeftHold()
        {
            return prevMouseState.LeftButton == ButtonState.Pressed &&
                   mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the user is holding the right mouse button
        /// </summary>
        /// <returns></returns>
        public bool RightHold()
        {
            return prevMouseState.RightButton == ButtonState.Pressed &&
                   mouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the user pressed the middle mouse button
        /// </summary>
        /// <returns></returns>
        public bool MiddleClick()
        {
            return prevMouseState.MiddleButton == ButtonState.Pressed &&
                   mouseState.MiddleButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the user pressed the middle mouse button. Returns true
        /// the moment the user presses the button
        /// </summary>
        /// <returns></returns>
        public bool MiddleClickImmediate()
        {
            return prevMouseState.MiddleButton == ButtonState.Released &&
                   mouseState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Gets the mouse rotation relative to the world
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public float MouseRotation(Camera2D origin)
        {
            Vector2 delta = MousePosition - Vector2.Transform(origin.Position, origin.Transform);

            return (float)Math.Round(Math.Atan2(delta.Y, delta.X), 2);
        }

        /// <summary>
        /// Gets the delta between the camera and the mouse
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public Vector2 Delta(Camera2D origin)
        {
            Vector2 delta = Vector2.Transform(MousePosition, origin.Transform) - origin.Position;
            return delta;
        }

        /// <summary>
        /// Converts the mouse coordinates from screen to world coordinates
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="origin"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 vector, Camera2D origin, Vector2 center)
        {
            return (((vector - center) / origin.ZoomFactor) + origin.Position);
        }


        /// <summary>
        /// Converts the mouse coordinates from the world to the screen
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public Vector2 WorldToScreen(Vector2 vector, Camera2D origin)
        {
            return (vector - origin.Position);
        }

        /// <summary>
        /// Gets text input
        /// </summary>
        /// <returns></returns>
        public string GetText(string text)
        {
            // Check if the capslock key was tapped
            if (Tapped(Keys.CapsLock))
            {
                capslock = !capslock;
            }

            // If one of the shift keys are being held down
            bool shift = Held(Keys.LeftShift) || Held(Keys.RightShift);

            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys.Where(Tapped))
            {
                // Special cases for either the backspace or space key
                switch (key)
                {
                    case Keys.Back:
                        text = text.Length > 1 ? text.Substring(0, text.Length - 1) : string.Empty;
                        break;
                    case Keys.Space:
                        text += ' ';
                        break;
                    case Keys.OemPeriod:
                        if (shift)
                        {
                            text += '>';
                        }
                        else
                        {
                            text += '.';
                        }
                        break;
                    case Keys.Decimal:
                        text += '.';
                        break;
                    case Keys.OemPlus:
                        if (shift)
                        {
                            text += '+';
                        }
                        else
                        {
                            text += '=';
                        }
                        break;
                    case Keys.Add:
                        text += '+';
                        break;
                    case Keys.OemMinus:
                        if (shift)
                        {
                            text += '_';
                        }
                        else
                        {
                            text += '-';
                        }
                        break;
                    case Keys.Subtract:
                        text += '-';
                        break;
                    case Keys.OemQuestion:
                        if (shift)
                        {
                            text += '?';
                        }
                        else
                        {
                            text += '/';
                        }
                        break;
                    case Keys.OemPipe:
                        if (shift)
                        {
                            text += '|';
                        }
                        else
                        {
                            text += '\\';
                        }
                        break;
                    case Keys.OemOpenBrackets:
                        if (shift)
                        {
                            text += '{';
                        }
                        else
                        {
                            text += '[';
                        }
                        break;
                    case Keys.OemCloseBrackets:
                        if (shift)
                        {
                            text += '}';
                        }
                        else
                        {
                            text += ']';
                        }
                        break;
                    case Keys.OemComma:
                        if (shift)
                        {
                            text += '<';
                        }
                        else
                        {
                            text += ',';
                        }
                        break;
                    case Keys.OemSemicolon:
                        if (shift)
                        {
                            text += ':';
                        }
                        else
                        {
                            text += ';';
                        }
                        break;
                    case Keys.OemQuotes:
                        if (shift)
                        {
                            text += '"';
                        }
                        else
                        {
                            text += '\'';
                        }
                        break;
                    case Keys.Multiply:
                        text += '*';
                        break;
                    case Keys.Divide:
                        text += '/';
                        break;
                    case Keys.OemTilde:
                        if (shift)
                        {
                            text += '~';
                        }
                        else
                        {
                            text += '`';
                        }
                        break;
                    default:
                    {
                        // Convert the key into a literal string
                        string asciiString = key.ToString();

                        // If the length is greater than one, it is either one of the
                        // function keys, special keys (Ctrl, Shift, Capslock, etc), or a key
                        // that is not part of the alphabet.
                        switch (asciiString.Length)
                        {
                            case 1: // A letter

                                // If capslock is enabled or shift is not held, 
                                // convert the character into lowercase
                                if (capslock || shift)
                                {
                                    text += asciiString;
                                }
                                else
                                {
                                    text += asciiString.ToLower();
                                }
                                break;
                            case 2: // NumPad Numbers

                                // If the first character is D (meaning a numpad number) and not one of the Function keys which start with F
                                if (asciiString[0] == 'D')
                                {
                                    // If 
                                    if (shift)
                                    {
                                        switch (asciiString[1])
                                        {
                                            case '1':
                                                text += '!';
                                                break;
                                            case '2':
                                                text += '@';
                                                break;
                                            case '3':
                                                text += '#';
                                                break;
                                            case '4':
                                                text += '$';
                                                break;
                                            case '5':
                                                text += '%';
                                                break;
                                            case '6':
                                                text += '^';
                                                break;
                                            case '7':
                                                text += '&';
                                                break;
                                            case '8':
                                                text += '*';
                                                break;
                                            case '9':
                                                text += '(';
                                                break;
                                            case '0':
                                                text += ')';
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        text += asciiString[1];
                                    }
                                }
                                break;
                            default:
                                if (asciiString.Contains("NumPad"))
                                {
                                    text += asciiString[6];
                                }
                                break;
                        }
                    }
                        break;
                }
            }
            return text;
        }
    }
}
