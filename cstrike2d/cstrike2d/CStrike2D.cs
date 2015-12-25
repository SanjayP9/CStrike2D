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
        private SpriteFont defFont;
        private InputManager inputManager;

        private Camera2D camera;

        private Texture2D pixelTexture;
        private Map newMap;


        private float glowTimer = 0f;
        private bool fadeIn = true;

        private int[] collidables;
        private int[] raycastCollidables;

        private Stopwatch stopWatch = new Stopwatch();
        private double pathfindingTime;

        private RenderTarget2D render;
        private RenderTarget2D finalRender;

        // Model and View
        private CStrikeModel model;
        private CStrikeRenderer view;

        // Game Properties (FPS, Resolution)

        /// <summary>
        /// The dimensions of the window
        /// </summary>
        public Vector2 Dimensions { get; }

        /// <summary>
        /// Returns the center coordinate of the window
        /// </summary>
        public Vector2 Center { get; }

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

            Dimensions = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Center = new Vector2(Dimensions.X / 2, Dimensions.Y / 2);

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
            model = new CStrikeModel();
            view = new CStrikeRenderer(this);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            defFont = Content.Load<SpriteFont>("font/defFont");
            inputManager = new InputManager();
            camera = new Camera2D();

            pixelTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] {Color.White});

            LoadMap("default");
            WriteMap("bigmap");
            camera.Position = new Vector2((newMap.TileMap.GetLength(0)/2)*64 + 32,
                (newMap.TileMap.GetLength(1)/2)*64 + 32);

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

            inputManager.Tick();

            model.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

            inputManager.Tock();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, null, null, null, null,
                camera.GetTransform(GraphicsDevice));

            view.Draw(spriteBatch, model);

            spriteBatch.End();
            counter++;
            base.Draw(gameTime);
        }

        public void LoadMap(string mapFolder)
        {
            if (File.Exists("map/" + mapFolder + "/tileData.txt"))
            {
                string[] data = File.ReadAllLines("map/" + mapFolder + "/tileData.txt");
                int width = Convert.ToInt32(data[0]);
                int height = Convert.ToInt32(data[1]);
                int[,] tiledata = new int[width, height];

                for (int y = 0; y < height; y++)
                {
                    string rowData = data[y + 2];
                    for (int x = 0; x < width; x++)
                    {
                        tiledata[x, y] = Convert.ToInt32(rowData[x]) - 48;
                    }
                }
                newMap = new Map(tiledata);
            }
        }

        public void WriteMap(string mapFolder)
        {
            if (!Directory.Exists("map/" + mapFolder))
            {
                Directory.CreateDirectory("map/" + mapFolder);
            }
            StreamWriter stream = File.CreateText("map/" + mapFolder + "/tiledata.txt");

            int[,] defMap =
            {
                {1, 1, 1, 1, 1},
                {1, 0, 0, 0, 1},
                {1, 0, 0, 0, 1},
                {1, 0, 0, 0, 1},
                {1, 1, 0, 1, 1},
                {1, 0, 0, 0, 1},
                {1, 1, 1, 1, 1}
            };


            stream.WriteLine(defMap.GetLength(1));
            stream.WriteLine(defMap.GetLength(0));

            for (int y = 0; y < defMap.GetLength(0); y++)
            {
                for (int x = 0; x < defMap.GetLength(1); x++)
                {
                    stream.Write(defMap[y, x]);
                }
                stream.WriteLine();
            }

            stream.Close();

            LoadMap("bigmap");
        }

        public int[] GetCollidableTiles(int origin)
        {
            List<int> tiles = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                int tile = SearchTile(origin, i);

                if (tile != -1)
                {
                    tiles.Add(tile);
                }
            }
            return tiles.ToArray();
        }

        public int SearchTile(int origin, int direction)
        {
            // Left Down Right Up
            // Top-Left Top-Right Bottom-Left Bottom-Right
            switch (direction)
            {
                    // Left
                case 0:
                    if ((origin - 1) > 0)
                    {
                        if (newMap.TypeFromTileNumber(origin - 1) == 0)
                        {
                            return SearchTile(origin - 1, direction);
                        }
                        return origin - 1;
                    }
                    break;
                    // Down
                case 1:
                    if ((origin + newMap.MaxCol) < newMap.MaxTiles)
                    {
                        if (newMap.TypeFromTileNumber(origin + newMap.MaxCol) == 0)
                        {
                            return SearchTile(origin + newMap.MaxCol, direction);
                        }
                        return (origin + newMap.MaxCol);
                    }
                    break;
                    // Right
                case 2:
                    if ((origin + 1) < newMap.MaxTiles)
                    {
                        if (newMap.TypeFromTileNumber(origin + 1) == 0)
                        {
                            return SearchTile(origin + 1, direction);
                        }
                        return origin + 1;
                    }
                    break;
                    // Up
                case 3:
                    if ((origin - (newMap.MaxCol)) > 0)
                    {
                        if (newMap.TypeFromTileNumber(origin - newMap.MaxCol) == 0)
                        {
                            return SearchTile(origin - newMap.MaxCol, direction);
                        }
                        return (origin - (newMap.MaxCol));
                    }
                    break;
                    // Top-Left
                case 4:
                    if ((origin - (newMap.MaxCol + 1)) >= 0)
                    {
                        if (newMap.TypeFromTileNumber(origin - (newMap.MaxCol + 1)) == 0)
                        {
                            return SearchTile(origin - (newMap.MaxCol + 1), direction);
                        }
                        return (origin - (newMap.MaxCol + 1));
                    }
                    break;
                    // Top-Right
                case 5:
                    if (origin - (newMap.MaxCol - 1) > 0)
                    {
                        if (newMap.TypeFromTileNumber(origin - (newMap.MaxCol - 1)) == 0)
                        {
                            return SearchTile(origin - (newMap.MaxCol - 1), direction);
                        }
                        return (origin - (newMap.MaxCol - 1));
                    }
                    break;
                    // Bottom-Left
                case 6:
                    if (origin + (newMap.MaxCol - 1) < newMap.MaxTiles)
                    {
                        if (newMap.TypeFromTileNumber(origin + (newMap.MaxCol - 1)) == 0)
                        {
                            return SearchTile(origin + (newMap.MaxCol - 1), direction);
                        }
                        return origin + (newMap.MaxCol - 1);
                    }
                    break;
                    // Bottom-Right
                case 7:
                    if (origin + (newMap.MaxCol + 1) < newMap.MaxTiles)
                    {
                        if (newMap.TypeFromTileNumber(origin + (newMap.MaxCol + 1)) == 0)
                        {
                            return SearchTile(origin + (newMap.MaxCol + 1), direction);
                        }
                        return origin + (newMap.MaxCol + 1);
                    }
                    break;
                default:
                    return -1;
            }
            return -1;
        }

        public int[] GetWalls(int origin, float angle)
        {
            angle = MathHelper.Clamp(angle, 0, 6);
            int[] rowCol = newMap.FromTile(origin);
            List<int> tiles = new List<int>();

            // 1 = Bottom-Right (PI / 2)
            // 2 = Bottom-Left (PI)
            // 3 = Top-Left (3 * PI / 2)
            // 4 = Top-Right (2 PI)
            switch (MathOps.ReturnQuadrant(angle))
            {
                case 1:
                    for (int row = rowCol[1]; row < newMap.MaxRow; row++)
                    {
                        for (int col = rowCol[0]; col < newMap.MaxCol; col++)
                        {
                            if (newMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(newMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
                case 2:
                    for (int row = rowCol[1]; row < newMap.MaxRow; row++)
                    {
                        for (int col = 0; col <= rowCol[0]; col++)
                        {
                            if (newMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(newMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
                case 3:
                    for (int row = 0; row <= rowCol[1]; row++)
                    {
                        for (int col = 0; col <= rowCol[0]; col++)
                        {
                            if (newMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(newMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
                case 4:
                    for (int row = 0; row <= rowCol[1]; row++)
                    {
                        for (int col = rowCol[0]; col < newMap.MaxCol; col++)
                        {
                            if (newMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(newMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
            }

            if (tiles.Count == 0)
            {
                throw new Exception("Rip skins");
            }

            return tiles.ToArray();
        }
    }
}
