using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CStrike2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cstrike2d
{
    public abstract class GUIComponent
    {

        /// <summary>
        /// The current state of the component
        /// </summary>
        public abstract State CurState { get; protected set; }

        /// <summary>
        /// The dimensions of the component
        /// </summary>
        public abstract Rectangle Dimensions { get; protected set; }

        /// <summary>
        /// Used to identify the component
        /// </summary>
        public abstract string Identifier { get; protected set; }

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
        /// Shows the component
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Hides the component
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// All logic operations of the component
        /// </summary>
        /// <param name="gameTime"> Uses walltime for calculations</param>
        public abstract void Update(float gameTime);

        /// <summary>
        /// All draw operations of the component
        /// </summary>
        /// <param name="sb"> Used to draw the object on the screen</param>
        /// <param name="assets"> Assets that may be used for drawing</param>
        public abstract void Draw(SpriteBatch sb, Assets assets);
    }
}
