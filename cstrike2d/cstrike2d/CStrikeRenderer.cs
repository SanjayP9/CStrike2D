// Author: Mark Voong
// File Name: View.cs
// Project: CStrike2D
// Date Created: Dec 6th 2015
// Date Modified: Dec 23rd 2015
// Description: Handles all graphical aspects of the game
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    class CStrikeRenderer
    {
        // Asset loaders

        /// <summary>
        /// Loads assets that are required at the start of the application (fonts, UI)
        /// </summary>
        private ContentManager coreContentLoader { get; set; }

        /// <summary>
        /// Loads assets that are specific to a map (tilemaps, particles)
        /// </summary>
        private ContentManager mapContentLoader { get; set; }

        /// <summary>
        /// Loads assets that are required in-game (weapons, playermodels, grenades
        /// </summary>
        private ContentManager gameContentLoader { get; set; }

        /// <summary>
        /// Initializes the view and loads all applicable assets
        /// </summary>
        /// <param name="instance">The driver class that contains the ServiceProvider</param>
        public CStrikeRenderer(Game instance)
        {
            // Initialize Content Loaders
            coreContentLoader = new ContentManager(instance.Services);
            mapContentLoader = new ContentManager(instance.Services);
            gameContentLoader = new ContentManager(instance.Services);


        }

        /// <summary>
        /// Unloads any content that is associated with the map
        /// </summary>
        public void UnloadMapContent()
        {
            mapContentLoader.Unload();
        }

        /// <summary>
        /// Unloads any content that is associated with the game
        /// </summary>
        public void UnloadGameContent()
        {
            gameContentLoader.Unload();
        }

        /// <summary>
        /// Unloads all content from the pipeline, use only when the game
        /// is shutting down
        /// </summary>
        public void UnloadAll()
        {
            coreContentLoader.Unload();
            mapContentLoader.Unload();
            gameContentLoader.Unload();
        }

        /// <summary>
        /// Draws all components of the game
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="model"></param>
        public void Draw(SpriteBatch sb, CStrikeModel model)
        {
            
        }
    }
}
