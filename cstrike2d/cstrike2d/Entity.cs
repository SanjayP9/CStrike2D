// Author: Mark Voong
// File Name: Entity.cs
// Project: Global Offensive
// Date Created: Dec 6th 2015
// Date Modified: Dec 23rd 2015
// Description: An object that is able to interact with the world
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public abstract class Entity
    {
        public abstract int DrawOrder { get; protected set; }

        /// <summary>
        /// The current position of the entity
        /// </summary>
        public abstract Vector2 Position { get; protected set; }

        /// <summary>
        /// The dimensions of the entity
        /// </summary>
        public abstract Rectangle Dimensions { get; protected set; }

        protected Assets Assets { get; set; }

        protected Entity(Assets assets)
        {
            Assets = assets;
        }

        /// <summary>
        /// Update logic of the entity
        /// </summary>
        /// <param name="gameTime"> Walltime. Used for calculations</param>
        public abstract void Update(float gameTime);

        /// <summary>
        /// Draw logic of the entity
        /// </summary>
        /// <param name="sb"> Spritebatch used to draw components of the game</param>
        public abstract void Draw(SpriteBatch sb);
    }
}