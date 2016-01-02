// Author: Mark Voong
// File Name: TextBox.cs
// Project Name: CStrike2D
// Creation Date: Jan 2nd, 2016
// Modified Date: Jan 2nd, 2016
// Description: Handles all input from the user

using System;
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    class TextBox : GUIComponent
    {
        public override State CurState { get; protected set; }
        public override string Identifier { get; protected set; }

        private Color textColour;
        private string text;
        private float animTime;
        private EasingFunctions.AnimationType animType;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Vector2 position;
        private float alpha;

        public TextBox(string identifier, Vector2 position, string text, Color textColour, float animTime, EasingFunctions.AnimationType animType, AnimationDirection animDir, Assets assets)
            : base(assets)
        {
            Identifier = identifier;
            this.textColour = textColour;
            this.text = text;
            this.position = position;
            this.animTime = animTime;
            this.animType = animType;
            endPosition = position;
            startPosition = SetStartPosition(animDir);
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
                    return new Vector2(1366, endPosition.Y);
                case AnimationDirection.Right:
                    return new Vector2(-250, endPosition.Y);
                case AnimationDirection.Up:
                    return new Vector2(endPosition.X, 768);
                case AnimationDirection.Down:
                    return new Vector2(endPosition.X, -250);
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
                        alpha += ALPHA_CHANGE;
                    }
                    break;
                case State.TransitionOut:
                    // Advance time
                    timer -= gameTime;

                    position.X = (float)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                    position.Y = (float)EasingFunctions.Animate(timer, startPosition.Y, endPosition.Y, animTime, animType);

                    if (timer >= animTime)
                    {
                        CurState = State.Active;
                    }

                    if (alpha >= 0.0f)
                    {
                        alpha -= ALPHA_CHANGE;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (CurState != State.InActive)
            {
                // Draw Text
                sb.DrawString(Assets.DefaultFont, text, position, textColour);
            }
        }
    }
}