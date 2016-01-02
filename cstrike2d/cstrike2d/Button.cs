// Author: Mark Voong
// Class Name: Button.cs
// Creation Date: Dec 23rd 2015
// Modified Date:
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
        /// The dimensions of the button
        /// </summary>
        public override Rectangle Dimensions { get; protected set; }

        /// <summary>
        /// Used to identify the button
        /// </summary>
        public override string Identifier { get; protected set; }

        private const float ALPHA_CHANGE = 0.05f;       // Rate at which the transparency changes per second
        private Vector2 startPosition;                  // The off-screen position of the button
        private Vector2 endPosition;                    // The final destination of the button
        private Vector2 textPosition;                   // Position of the text
        private Color fillColour;                       // The colour used for the fill of the button
        private Color borderColour;                     // The colour used for the border of the button
        private Color textColour;                       // The colour used for the text
        private string text;                            // The text shown inside the button
        private float alpha = 0.0f;                     // The alpha transparency of the button
        private float timer = 0.0f;                     // Timer used to animate the button
        private float animTime;                         // Time the button takes to move from one point to another
        private EasingFunctions.AnimationType animType; // Type of animation the button should use

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
        /// <param name="assets"></param>
        public Button(string identifier, Rectangle dimensions, Color fillColour, Color borderColour, Color textColour,
            string text, float animTime, EasingFunctions.AnimationType animType, Assets assets) : base(assets)
        {
            Identifier = identifier;
            Dimensions = dimensions;
            this.fillColour = fillColour;
            this.borderColour = borderColour;
            this.textColour = textColour;
            this.text = text;
            this.animTime = animTime;
            this.animType = animType;
            CurState = State.InActive;
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
        /// <param name="assets"></param>
        public Button(string identifier, Rectangle dimensions, Color textColour, string text, float animTime,
            EasingFunctions.AnimationType animType, Assets assets) : base(assets)
        {
            Identifier = identifier;
            Dimensions = dimensions;
            this.textColour = textColour;
            this.text = text;
            this.animTime = animTime;
            this.animType = animType;
            CurState = State.InActive;
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
                    Dimensions.Offset(
                        (int) EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType),
                        (int) EasingFunctions.Animate(timer, startPosition.Y, endPosition.Y, animTime, animType));

                    // If the button has reached its end point, set it to active
                    if (Dimensions.X == (int) endPosition.X &&
                        Dimensions.Y == (int) endPosition.Y)
                    {
                        CurState = State.Active;
                    }

                    // Change the alpha
                    if (alpha <= 1.0f)
                    {
                        alpha += ALPHA_CHANGE * gameTime;
                    }
                    break;
                case State.TransitionOut:

                    // Advance time
                    timer += gameTime;

                    // Move the button
                    Dimensions.Offset(
                        (int)EasingFunctions.Animate(timer, endPosition.X, startPosition.X, animTime, animType),
                        (int)EasingFunctions.Animate(timer, endPosition.Y, startPosition.Y, animTime, animType));

                    // If the button has reached its end point, set it to active
                    if (Dimensions.X == (int)startPosition.X &&
                        Dimensions.Y == (int)startPosition.Y)
                    {
                        CurState = State.InActive;
                    }

                    // Change the alpha
                    if (alpha >= 0.0f)
                    {
                        alpha -= ALPHA_CHANGE * gameTime;
                    }
                    break;
            }
        }

        /// <summary>
        /// Transitions in the button
        /// </summary>
        public override void Show()
        {
            // Reset the timer only if the button isn't already currently moving
            if (CurState == State.InActive)
            {
                timer = 0;
            }
            CurState = State.TransitionIn;
        }

        /// <summary>
        /// Transitions out the button
        /// </summary>
        public override void Hide()
        {
            // Reset the timer only if the button isn't already currently moving
            if (CurState == State.Active)
            {
                timer = 0;
            }
            CurState = State.TransitionOut;
        }

        /// <summary>
        /// Checks if the button is hovered over
        /// </summary>
        /// <returns></returns>
        public bool Hover(InputManager input)
        {
            return Dimensions.Contains((int)input.MousePosition.X, (int)input.MousePosition.Y);
        }

        /// <summary>
        /// Check if the button was clicked
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Clicked(InputManager input)
        {
            return Hover(input) && input.LeftClick();
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
                sb.Draw(Assets.PixelTexture, Dimensions, fillColour);

                // Draw text
                Vector2 centeredText = new Vector2(
                    Dimensions.Center.X - (Assets.DefaultFont.MeasureString(text).X / 2),
                    Dimensions.Center.Y - (Assets.DefaultFont.MeasureString(text).Y / 2));

                sb.DrawString(Assets.DefaultFont, text, centeredText, textColour, 0, Vector2.Zero, 1f,
                    SpriteEffects.None, 0);

                // Draw border

                // Draw left side
                sb.Draw(Assets.PixelTexture, new Rectangle(Dimensions.Left, Dimensions.Y, 1, Dimensions.Height),
                    borderColour);
                // Draw right side
                sb.Draw(Assets.PixelTexture, new Rectangle(Dimensions.Right, Dimensions.Y, 1, Dimensions.Height),
                    borderColour);
                // Draw bottom side
                sb.Draw(Assets.PixelTexture, new Rectangle(Dimensions.X, Dimensions.Bottom, Dimensions.Width, 1),
                    borderColour);
                // Draw top side
                sb.Draw(Assets.PixelTexture, new Rectangle(Dimensions.X, Dimensions.Y, Dimensions.Width, 1),
                    borderColour);
            }
        }
    }
}