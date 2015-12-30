// Author: Mark Voong
// Class Name: Assets.cs
// Creation Date: Dec 23rd 2015
// Modified Date:
// Description: Stores all assets required in the game and is globally accessible
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    /// <summary>
    /// Contains all assets that are required in the game
    /// </summary>
    public class Assets
    {
        // Assets
        public Texture2D PixelTexture { get; private set; }     // Pixel texture
        public SpriteFont DefaultFont { get; private set; }     // Default Font

        public Texture2D CTMenuBackground { get; private set; } // Counter-Terrorist Menu Background
        public Texture2D TMenuBackground { get; private set; }  // Terrorist Menu Background

        /// <summary>
        /// Loads assets that are required at the start of the application (fonts, UI)
        /// </summary>
        private ContentManager coreContentLoader;

        /// <summary>
        /// Loads assets that are specific to a map (tilemaps, particles)
        /// </summary>
        private ContentManager mapContentLoader;

        /// <summary>
        /// Loads assets that are required in-game (weapons, playermodels, grenades
        /// </summary>
        private ContentManager gameContentLoader;

        public Assets(Game instance)
        {
            // Initialize Content Loaders
            coreContentLoader = new ContentManager(instance.Services);
            mapContentLoader = new ContentManager(instance.Services);
            gameContentLoader = new ContentManager(instance.Services);

            coreContentLoader.RootDirectory = "Content";
            mapContentLoader.RootDirectory = "Content";
            gameContentLoader.RootDirectory = "Content";

            // Load Core Content
            LoadCoreContent(instance);
        }

        /// <summary>
        /// Load core assets in this method
        /// </summary>
        public void LoadCoreContent(Game instance)
        {
            // Load Pixel texture
            PixelTexture = new Texture2D(instance.GraphicsDevice, 1, 1);
            PixelTexture.SetData(new [] {Color.White});

            DefaultFont = coreContentLoader.Load<SpriteFont>("font/defFont");
            CTMenuBackground = coreContentLoader.Load<Texture2D>("texture/bg/ctmenu");
            TMenuBackground = coreContentLoader.Load<Texture2D>("texture/bg/tmenu");
        }

        /// <summary>
        /// Load map assets in this method
        /// </summary>
        public void LoadMapContent()
        {

        }

        /// <summary>
        /// Load game assets in this method
        /// </summary>
        public void LoadGameContent()
        {

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
    }
}
