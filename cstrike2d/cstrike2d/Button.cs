// Author: Mark Voong
// File Name: Button.cs
// Project Name: Global Offensive
// Creation Date: Dec 23rd, 2015
// Modified Date: Jan 8th, 2016
// Description: Creates a button, which can be clicked
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public sealed class Button : GUIComponent
    {
        public override State CurState { get; protected set; }

        /// <summary>
        /// Used to identify the button
        /// </summary>
        public override string Identifier { get; protected set; }

        private Vector2 startPosition;                  // The off-screen position of the button
        private Vector2 endPosition;                    // The final destination of the button
        private Vector2 textPosition;                   // Position of the text
        private Color fillColour;                       // The colour used for the fill of the button
        private Color borderColour;                     // The colour used for the border of the button
        private Color textColour;                       // The colour used for the text
        private string text;                            // The text shown inside the button
        private float alpha = 0.0f;                     // The alpha transparency of the button
        private float changeRate;
        private float animTime;                         // Time the button takes to move from one point to another
        private EasingFunctions.AnimationType animType; // Type of animation the button should use

        private bool debug = true;

        /// <summary>
        /// Creates a button
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="dimensions"></param>
        /// <param name="fillColour"></param>
        /// <param name="borderColour"></param>
        /// <param name="textColour"></param>
        /// <param name="text"></param>
        /// <param name="animTime"></param>
        /// <param name="animType"></param>
        /// <param name="animDir"></param>
        /// <param name="assets"></param>
        public Button(string identifier, Rectangle dimensions, Color fillColour, Color borderColour, Color textColour,
            string text, float animTime, EasingFunctions.AnimationType animType, AnimationDirection animDir, Assets assets) : base(assets)
        {
            Identifier = identifier;
            this.fillColour = fillColour;
            this.borderColour = borderColour;
            this.textColour = textColour;
            this.text = text;
            this.animTime = animTime;
            this.animType = animType;
            changeRate = 1f / animTime;
            CurState = State.InActive;
            endPosition = new Vector2(dimensions.X, dimensions.Y);
            startPosition = SetStartPosition(animDir);
            this.dimensions.Location = new Point((int)startPosition.X, (int)startPosition.Y);
            this.dimensions.Width = dimensions.Width;
            this.dimensions.Height = dimensions.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="dimensions"></param>
        /// <param name="textColour"></param>
        /// <param name="text"></param>
        /// <param name="animTime"></param>
        /// <param name="animType"></param>
        /// <param name="animDir"></param>
        /// <param name="assets"></param>
        public Button(string identifier, Rectangle dimensions, Color textColour, string text, float animTime,
            EasingFunctions.AnimationType animType, AnimationDirection animDir, Assets assets) : base(assets)
        {
            Identifier = identifier;
            this.textColour = textColour;
            this.text = text;
            this.animTime = animTime;
            this.animType = animType;
            changeRate = 1f / animTime;
            CurState = State.InActive;
            endPosition = new Vector2(dimensions.X, dimensions.Y);
            startPosition = SetStartPosition(animDir);
            this.dimensions.Location = new Point((int)startPosition.X, (int)startPosition.Y);
            this.dimensions.Width = dimensions.Width;
            this.dimensions.Height = dimensions.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animDir"></param>
        /// <returns></returns>
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
                case AnimationDirection.None:
                    return endPosition;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Updates all logic for the button
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(float gameTime)
        {
            switch (CurState)
            {
                case State.TransitionIn:

                    // Advance time
                    timer += gameTime;

                    // Move the button
                    dimensions.X =
                        (int) EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                    dimensions.Y =
                        (int) EasingFunctions.Animate(timer, startPosition.Y, endPosition.Y, animTime, animType);
                    
                    // If the button has reached its end point, set it to active
                    if (timer >= animTime)
                    {
                        CurState = State.Active;
                    }

                    // Change the alpha
                    if (alpha <= 1.0f)
                    {
                        alpha += changeRate * gameTime;
                    }
                    break;
                case State.TransitionOut:

                    // Advance time
                    timer -= gameTime;

                    // Move the button
                    dimensions.X =
                        (int)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                    dimensions.Y =
                        (int)EasingFunctions.Animate(timer, startPosition.Y, endPosition.Y, animTime, animType);

                    // If the button has reached its end point, set it to active
                    if (timer <= 0.0f)
                    {
                        CurState = State.InActive;
                    }

                    // Change the alpha
                    if (alpha >= 0.0f)
                    {
                        alpha -= changeRate * gameTime;
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws the button
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="assets"></param>
        public override void Draw(SpriteBatch sb)
        {
            if (CurState != State.InActive)
            {
                // Draw fill
                sb.Draw(Assets.PixelTexture, dimensions, fillColour);

                // Draw text
                Vector2 centeredText = new Vector2(
                    dimensions.Center.X - (Assets.DefaultFont.MeasureString(text).X / 2),
                    dimensions.Center.Y - (Assets.DefaultFont.MeasureString(text).Y / 2));

                sb.DrawString(Assets.DefaultFont, text, centeredText, textColour * alpha, 0, Vector2.Zero, 1f,
                    SpriteEffects.None, 0);

                // Draw border

                if (debug)
                {
                    // Draw left side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.Left, dimensions.Y, 1, dimensions.Height),
                        Color.Red * alpha);
                    // Draw right side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.Right, dimensions.Y, 1, dimensions.Height),
                        Color.Red * alpha);
                    // Draw bottom side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.X, dimensions.Bottom, dimensions.Width, 1),
                        Color.Red * alpha);
                    // Draw top side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, 1),
                        Color.Red * alpha);
                }
                else
                {
                    // Draw left side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.Left, dimensions.Y, 1, dimensions.Height),
                        borderColour * alpha);
                    // Draw right side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.Right, dimensions.Y, 1, dimensions.Height),
                        borderColour * alpha);
                    // Draw bottom side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.X, dimensions.Bottom, dimensions.Width, 1),
                        borderColour * alpha);
                    // Draw top side
                    sb.Draw(Assets.PixelTexture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, 1),
                        borderColour * alpha);
                }
            }
        }

        /// <summary>
        /// Checks if the button is hovered over
        /// </summary>
        /// <returns></returns>
        public bool Hover(InputManager input)
        {
            return CurState == State.Active && dimensions.Contains((int) input.MousePosition.X, (int) input.MousePosition.Y);
        }

        /// <summary>
        /// Check if the button was clicked
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Clicked(InputManager input)
        {
            if (CurState == State.Active)
            {
                return Hover(input) && input.LeftClick();
            }
            return false;
        }
    }
}