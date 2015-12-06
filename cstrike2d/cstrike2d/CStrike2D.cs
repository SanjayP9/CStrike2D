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

namespace cstrike2d
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

        // FPS
        private int counter;
        private float fps;
        private float timer;

        private Texture2D pixelTexture;
        private Map newMap;

        private Vector2 center;

        private float glowTimer = 0f;
        private bool fadeIn = true;

        private int[] collidables;
        private int[] raycastCollidables;

        private Stopwatch stopWatch = new Stopwatch();
        private double pathfindingTime;

        private RenderTarget2D render;
        private RenderTarget2D finalRender;

        public CStrike2D()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;



            center = new Vector2(graphics.PreferredBackBufferWidth/2f,
                graphics.PreferredBackBufferHeight/2f);

            graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
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

            render = new RenderTarget2D(graphics.GraphicsDevice, 1366, 768, false, 
                SurfaceFormat.Rgba64, DepthFormat.Depth24, 24, RenderTargetUsage.DiscardContents);

            finalRender = new RenderTarget2D(graphics.GraphicsDevice, 1366, 768, false,
                SurfaceFormat.Rgba64, DepthFormat.Depth24, 24, RenderTargetUsage.DiscardContents);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            counter++;
            inputManager.Tick();

            if (fadeIn)
            {
                glowTimer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

                if (glowTimer >= 1000f)
                {
                    fadeIn = !fadeIn;
                }
            }
            else
            {
                glowTimer -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;

                if (glowTimer <= 0f)
                {
                    fadeIn = !fadeIn;
                }
            }

            if (timer >= 100f)
            {
                timer = 0;
                fps = counter*10f;
                counter = 0;
            }

            if (inputManager.Held(Keys.A))
            {
                camera.Position.X -= 5f;
            }

            if (inputManager.Held(Keys.D))
            {
                camera.Position.X += 5f;
            }

            if (inputManager.Held(Keys.W))
            {
                camera.Position.Y -= 5f;
            }

            if (inputManager.Held(Keys.S))
            {
                camera.Position.Y += 5f;
            }

            stopWatch.Start();
            collidables = GetCollidableTiles(newMap.ToTile(camera.Col(), camera.Row()));
            stopWatch.Stop();

            raycastCollidables = GetWalls(newMap.ToTile(camera.Col(), camera.Row()), (24 * (glowTimer / 1000f)) * (float)Math.PI / 12f);

            pathfindingTime = Math.Round((double)stopWatch.ElapsedMilliseconds, 10);

            stopWatch.Reset();

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
            GraphicsDevice.SetRenderTarget(render);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, null, null, null, null,
                camera.GetTransform(GraphicsDevice));

            for (int col = 0; col < newMap.MaxCol; col++)
            {
                for (int row = 0; row < newMap.MaxRow; row++)
                {
                    Rectangle rect = new Rectangle(col*64, row*64, 64, 64);

                        switch (newMap.TileMap[col, row].TileType)
                        {
                            case 1:
                                if (raycastCollidables.Contains(newMap.ToTile(col, row)))
                                {
                                    spriteBatch.Draw(pixelTexture, rect, Color.Gray);
                                    spriteBatch.Draw(pixelTexture, rect,
                                    Color.Red * (0.6f * (glowTimer / 1000f)));
                                }
                                else
                                {
                                    spriteBatch.Draw(pixelTexture, rect, Color.Green);
                                }
                                
                                break;
                            case 0:
                                spriteBatch.Draw(pixelTexture, rect, Color.Gold);
                                break;
                        }
                    
                    spriteBatch.DrawString(defFont, col + ", " + row + "\n" + ((row * newMap.MaxCol) + col), new Vector2(rect.Center.X - 15, rect.Center.Y - 15), Color.White);
                }
            }

            // Camera center
            spriteBatch.Draw(pixelTexture,
                new Rectangle((int) (camera.Position.X - 5), (int) (camera.Position.Y - 5), 10, 10), Color.Red);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, null, null, null, null,
                camera.GetTransform(GraphicsDevice));

            for (int x = 0; x <= newMap.TileMap.GetLength(0); x++)
            {
                for (int y = 0; y <= newMap.TileMap.GetLength(1); y++)
                {
                    spriteBatch.Draw(pixelTexture, new Rectangle(0, y * 64, 64 * x, 1), Color.Black);
                    spriteBatch.Draw(pixelTexture, new Rectangle(x * 64, 0, 1, 64 * y), Color.Black);
                }
            }

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(defFont,
                "Mouse (Local): " + inputManager.MousePosition + "\n" +
                "Mouse (World): " + inputManager.ScreenToWorld(inputManager.MousePosition, camera, center) + "\n" +
                "Camera (World): " + camera.Position + "\n" +
                "Mouse (Row): " + inputManager.GetRow(camera, center) + "\n" +
                "Mouse (Column): " + inputManager.GetColumn(camera, center) + "\n" +
                "Quadrant of Direction: " +
                MathOps.ReturnQuadrant(MathOps.RealRadians(MathOps.Angle(inputManager.MousePosition, center))) + "\n" +
                "Angle (Degrees): " + MathOps.ToDegrees(MathOps.Angle(inputManager.MousePosition, center)) + "\n" +
                "Angle (Radians): " + MathOps.RealRadians(MathOps.Angle(inputManager.MousePosition, center)) + "\n" + 
                "Location (Col, Row): " + (int)(camera.Position.X / 64) + ", " + (int)(camera.Position.Y / 64) + "\n" +
                "Collidable Face Calculation Time: " + pathfindingTime + "ms" + "\n" +
                "Check Angle: " + ((24 * (glowTimer / 1000f)) * (float)Math.PI) / 12f,
                Vector2.Zero, Color.White);

            // Center
            spriteBatch.Draw(pixelTexture, new Rectangle((int) center.X, (int) center.Y,
                (int) (MathOps.Delta(inputManager.MousePosition, center).Length()),
                2), null, Color.Red, (MathOps.Angle(inputManager.MousePosition, center)), Vector2.Zero,
                SpriteEffects.None, 0);

            // FPS
            spriteBatch.DrawString(defFont, "FPS: " + fps, new Vector2(graphics.PreferredBackBufferWidth - (defFont.MeasureString("FPS: " + fps).X), 0), Color.White);

            // Ray
            for (int i = 0; i < 24; i++)
            {
                spriteBatch.Draw(pixelTexture,
                    new Rectangle((int) center.X, (int) center.Y,
                        (int)
                            MathOps.Delta(center, MathOps.AngleToVector(i * (float)Math.PI/12f))
                                .Length(),
                        1), null, Color.Red,
                    (i * (float)Math.PI/12f), Vector2.Zero, SpriteEffects.None, 0);
            }


            // Mouse Point
            spriteBatch.Draw(pixelTexture,
                new Rectangle((int) inputManager.MousePosition.X, (int) inputManager.MousePosition.Y, 10, 10), null,
                Color.Green, 0, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(finalRender);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.CreateScale(2f));
            spriteBatch.Draw(render, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.CreateScale(0.5f));
            spriteBatch.Draw(finalRender, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
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
