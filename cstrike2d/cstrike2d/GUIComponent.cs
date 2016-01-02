using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public abstract class GUIComponent
    {

        /// <summary>
        /// The current state of the component
        /// </summary>
        public abstract State CurState { get; protected set; }

        protected Rectangle dimensions;

        /// <summary>
        /// Used to identify the component
        /// </summary>
        public abstract string Identifier { get; protected set; }

        protected float timer = 0.0f;                     // Timer used to animate the button
        protected Assets Assets;


        protected GUIComponent(Assets assets)
        {
            Assets = assets;
        }

        public Rectangle Dimensions()
        {
            return dimensions;
        }

        /// <summary>
        /// Different states the component could be in
        /// </summary>
        public enum State
        {
            InActive,
            TransitionIn,
            TransitionOut,
            Active
        }

        public enum AnimationDirection
        {
            Left,
            Right,
            Up,
            Down,
        }

        /// <summary>
        /// Shows the component
        /// </summary>
        public virtual void Show()
        {
            // Reset the timer only if the button isn't already currently moving
            if (CurState == State.InActive)
            {
                timer = 0;
            }
            CurState = State.TransitionIn;
        }

        /// <summary>
        /// Hides the component
        /// </summary>
        public virtual void Hide()
        {
            // Reset the timer only if the button isn't already currently moving
            if (CurState == State.Active)
            {
                timer = 0;
            }
            CurState = State.TransitionOut;
        }

        /// <summary>
        /// All logic operations of the component
        /// </summary>
        /// <param name="gameTime"> Uses walltime for calculations</param>
        public abstract void Update(float gameTime);

        /// <summary>
        /// All draw operations of the component
        /// </summary>
        /// <param name="sb"> Used to draw the object on the screen</param>
        public abstract void Draw(SpriteBatch sb);
    }
}