using System;
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public sealed class Bar : GUIComponent
    {
        public override State CurState { get; protected set; }
        public override string Identifier { get; protected set; }

        private const float ALPHA_CHANGE = 0.05f;
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
                    timer += gameTime;

                    // Move the bar
                    dimensions.Width =
                        (int)EasingFunctions.Animate(timer, endPosition.X, startPosition.X, animTime, animType);
                    dimensions.Height =
                        (int)EasingFunctions.Animate(timer, endPosition.Y, startPosition.Y, animTime, animType);

                    if (dimensions.Width == (int)(startPosition.X - endPosition.X) &&
                        dimensions.Height == (int)(startPosition.Y - endPosition.Y))
                    {
                        CurState = State.Active;
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
