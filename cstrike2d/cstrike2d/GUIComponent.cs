// Author: Mark Voong
// File Name: GUIComponent.cs
// Project Name: CStrike2D
// Creation Date: Dec 21st, 2015
// Modified Date: Jan 2nd, 2016
// Description: Base class for a UI component. Has basic functions for
// Animation and state
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assets"></param>
        protected GUIComponent(Assets assets)
        {
            Assets = assets;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Rectangle Dimensions()
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

        /// <summary>
        /// 
        /// </summary>
        public enum AnimationDirection
        {
            Left,
            Right,
            Up,
            Down,
            None
        }

        /// <summary>
        /// Shows the component
        /// </summary>
        public virtual void Show()
        {
            if (CurState != State.Active)
            {
                CurState = State.TransitionIn;
            }
        }

        /// <summary>
        /// Hides the component
        /// </summary>
        public virtual void Hide()
        {
            if (CurState != State.InActive)
            {
                CurState = State.TransitionOut;
            }
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