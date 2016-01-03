// Author: Mark Voong
// File Name: TextBox.cs
// Project Name: CStrike2D
// Creation Date: Jan 2nd, 2016
// Modified Date: Jan 2nd, 2016
// Description: Handles all input from the user

using System;
using System.Net.NetworkInformation;
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    sealed class TextBox : GUIComponent
    {
        public override State CurState { get; protected set; }
        public override string Identifier { get; protected set; }

        private Color textColour;
        private string text;
        private string data;
        private float animTime;
        private float changeRate;
        private EasingFunctions.AnimationType animType;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Vector2 position;
        private float alpha;

        /// <summary>
        /// Creates an animated text. Note the text is passed by value
        /// and will not change regardless of what happens to it. It is
        /// recommended you use the referenced version of this constructor
        /// to take advantage of persistent data
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="textColour"></param>
        /// <param name="animTime"></param>
        /// <param name="animType"></param>
        /// <param name="animDir"></param>
        /// <param name="assets"></param>
        public TextBox(string identifier, Vector2 position, string text, Color textColour, float animTime,
            EasingFunctions.AnimationType animType, AnimationDirection animDir, Assets assets)
            : base(assets)
        {
            Identifier = identifier;
            this.textColour = textColour;
            this.text = text;
            this.position = position;
            this.animTime = animTime;
            this.animType = animType;
            changeRate = 0.1f/animTime;
            endPosition = position;
            startPosition = SetStartPosition(animDir);
            this.position = startPosition;
        }

        /// <summary>
        /// Creates a text box with the text being a direct reference to another value.
        /// Useful for data that is constantly changing.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="position"></param>
        /// <param name="prefix"> Static string that is added to the beginning of the data</param>
        /// <param name="data"></param>
        /// <param name="textColour"></param>
        /// <param name="animTime"></param>
        /// <param name="animType"></param>
        /// <param name="animDir"></param>
        /// <param name="assets"></param>
        public TextBox(string identifier, Vector2 position, string prefix, ref Type data, Color textColour, float animTime,
            EasingFunctions.AnimationType animType, AnimationDirection animDir, Assets assets)
            : base(assets)
        {
            Identifier = identifier;
            this.textColour = textColour;
            text = prefix;
            this.data = data.ToString();
            this.position = position;
            this.animTime = animTime;
            this.animType = animType;
            changeRate = 0.1f/animTime;
            endPosition = position;
            startPosition = SetStartPosition(animDir);
            this.position = startPosition;
        }

        /// <summary>
        /// Returns the position of the textbox
        /// </summary>
        /// <returns></returns>
        public Vector2 Position()
        {
            return position;
        }

        public Vector2 SetStartPosition(AnimationDirection animDir)
        {
            switch (animDir)
            {
                case AnimationDirection.Left:
                    return new Vector2(endPosition.X + 1366, endPosition.Y);
                case AnimationDirection.Right:
                    return new Vector2(endPosition.X - 1366, endPosition.Y);
                case AnimationDirection.Up:
                    return new Vector2(endPosition.X, endPosition.Y + 768);
                case AnimationDirection.Down:
                    return new Vector2(endPosition.X, endPosition.Y - 768);
                default:
                    return endPosition;
            }
        }

        public override void Update(float gameTime)
        {
            switch (CurState)
            {
                case State.TransitionIn:

                    // Advance time
                    timer += gameTime;

                    position.X = (float)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                    position.Y = (float)EasingFunctions.Animate(timer, startPosition.Y, endPosition.Y, animTime, animType);

                    if (timer >= animTime)
                    {
                        CurState = State.Active;
                    }

                    if (alpha <= 1.0f)
                    {
                        alpha += changeRate * gameTime;
                    }
                    break;
                case State.TransitionOut:
                    // Advance time
                    timer -= gameTime;

                    position.X = (float)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                    position.Y = (float)EasingFunctions.Animate(timer, startPosition.Y, endPosition.Y, animTime, animType);

                    if (timer <= 0.0f)
                    {
                        CurState = State.InActive;
                    }

                    if (alpha >= 0.0f)
                    {
                        alpha -= changeRate * gameTime;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (CurState != State.InActive)
            {
                // Draw Text
                sb.DrawString(Assets.DefaultFont, text + data, position, textColour);
            }
        }

        public override void Show()
        {
            base.Show();
            if (position.X <= -100)
            {
                timer = 0;
            }
        }
    }
}