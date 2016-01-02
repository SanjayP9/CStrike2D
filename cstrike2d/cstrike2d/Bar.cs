// Author: Mark Voong
// File Name: View.cs
// Project: CStrike2D
// Date Created: Dec 6th 2015
// Date Modified: Jan 3rd 2016
// Description: Handles all graphical aspects of the game
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public sealed class Bar : GUIComponent
    {
        public override State CurState { get; protected set; }
        public override string Identifier { get; protected set; }

        private Vector2 startPosition;
        private Vector2 endPosition;
        private Color fillColour;
        private float alpha;
        private float maxAlpha;
        private float animTime;
        private EasingFunctions.AnimationType animType;
        private AnimationDirection animDir;

        public Bar(string identifier, Rectangle dimensions, float animTime, float maxAlpha, Color fillColour,
             EasingFunctions.AnimationType animType, AnimationDirection animDir, Assets assets)
            : base(assets)
        {
            Identifier = identifier;
            this.fillColour = fillColour;
            this.animTime = animTime;
            this.animType = animType;
            this.maxAlpha = maxAlpha;
            this.dimensions = dimensions;
            this.animDir = animDir;
            SetPosition(animDir);
            CurState = State.InActive;
        }

        public void SetPosition(AnimationDirection animDir)
        {
            switch (animDir)
            {
                case AnimationDirection.Left:
                    startPosition = new Vector2(dimensions.Right, dimensions.Top);
                    endPosition = new Vector2(dimensions.X, dimensions.Y);
                    break;
                case AnimationDirection.Right:
                    startPosition = new Vector2(dimensions.X, dimensions.Y);
                    endPosition = new Vector2(dimensions.Right, dimensions.Top);
                    break;
                case AnimationDirection.Up:
                    startPosition = new Vector2(dimensions.X, dimensions.Height);
                    endPosition = new Vector2(dimensions.X, dimensions.Y);
                    break;
                case AnimationDirection.Down:
                    startPosition = new Vector2(dimensions.X, dimensions.Y);
                    endPosition = new Vector2(dimensions.X, dimensions.Height);
                    break;
            }
        }

        public override void Update(float gameTime)
        {
            switch (CurState)
            {
                case State.TransitionIn:
                    timer += gameTime;

                    switch (animDir)
                    {
                        case AnimationDirection.Left:
                            dimensions.X =
                                (int)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                            break;
                        case AnimationDirection.Right:
                            dimensions.Width =
                                (int)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                            break;
                        case AnimationDirection.Up:
                            dimensions.Height =
                                (int)EasingFunctions.Animate(timer, endPosition.Y, startPosition.Y, animTime, animType);
                            break;
                        case AnimationDirection.Down:
                            dimensions.Height =
                               (int)EasingFunctions.Animate(timer, 0, endPosition.Y, animTime, animType);
                            break;
                    }

                    if (timer >= animTime)
                    {
                        CurState = State.Active;
                    }

                    // Move the bar

                    //dimensions.Height =
                    //    (int)EasingFunctions.Animate(timer, startPosition.Y, -startPosition.Y, animTime, animType);

                    // Change the alpha
                    if (alpha <= maxAlpha)
                    {
                        alpha += ALPHA_CHANGE;
                    }
                    break;
                case State.TransitionOut:
                    timer -= gameTime;

                    // Move the bar
                    switch (animDir)
                    {
                        case AnimationDirection.Left:
                            dimensions.X =
                                (int)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                            break;
                        case AnimationDirection.Right:
                            dimensions.Width =
                                (int)EasingFunctions.Animate(timer, startPosition.X, endPosition.X, animTime, animType);
                            break;
                        case AnimationDirection.Up:
                            dimensions.Height =
                                (int)EasingFunctions.Animate(timer, endPosition.Y, startPosition.Y, animTime, animType);
                            break;
                        case AnimationDirection.Down:
                            dimensions.Height =
                               (int)EasingFunctions.Animate(timer, 0, endPosition.Y, animTime, animType);
                            break;
                    }

                    if (timer <= 0)
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

        public override void Draw(SpriteBatch sb)
        {
            if (CurState != State.InActive)
            {
                sb.Draw(Assets.PixelTexture, dimensions, fillColour * alpha);
            }
        }
    }
}
