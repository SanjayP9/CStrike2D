using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CStrike2D : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private RenderTarget2D render;
        private RenderTarget2D finalRender;

        // Model and View
        private CStrikeModel model;
        private CStrikeRenderer view;

        // Game Properties (FPS, Resolution)

        /// <summary>
        /// The dimensions of the window
        /// </summary>
        private Vector2 dimensions;

        /// <summary>
        /// Returns the center coordinate of the window
        /// </summary>
        private Vector2 center;

        /// <summary>
        /// The number of screen updates in Frames Per Second
        /// </summary>
        public decimal FPS { get; private set; }

        private int counter;       // Used to count how many times the screen is drawn
        private decimal timer;     // Used to track 1 second intervals in walltime for counting FPS

        public CStrike2D()
        {
            graphics = new GraphicsDeviceManager(this);

            // Default dimensions
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;

            dimensions = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            center = new Vector2(dimensions.X / 2, dimensions.Y / 2);

            // Disable VSync
            graphics.SynchronizeWithVerticalRetrace = false;

            // Show the mouse
            IsMouseVisible = true;

            // Prefer Multi-Sampling
            graphics.PreferMultiSampling = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize Model and View
            model = new CStrikeModel(this, center, dimensions);
            view = new CStrikeRenderer(this);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unloads content from all pipelines
            view.UnloadAll();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timer += (decimal)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer >= 100m)
            {
                timer = 0;
                FPS = counter * 10m;
                counter = 0;
            }

            model.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Transparent);

            view.Draw(spriteBatch, model);

            counter++;
            base.Draw(gameTime);
        }
    }
}
