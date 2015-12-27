// Author: Mark Voong
// Class Name: CStrikeModel.cs
// Project: CStrike2D
// Creation Date: Dec 21st 2015
// Modified Date:
// Description: Handles all logic processing of the game

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using cstrike2d;
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class CStrikeModel
    {
        public UIManager InterfaceManager { get; private set; }

        private Camera2D camera;

        private Map newMap;
        public InputManager Input { get; private set; }


        private float glowTimer = 0f;
        private bool fadeIn = true;

        private int[] collidables;
        private int[] raycastCollidables;

        private Stopwatch stopWatch = new Stopwatch();
        private double pathfindingTime;

        private int counter;       // Used to count how many times the screen is drawn
        private decimal timer;     // Used to track 1 second intervals in walltime for counting FPS

        public Game DriverInstance { get; private set; }

        public enum State
        {
            Menu,
            Options,
            Lobby,
            InGame
        }

        public State CurState { get; private set; }

        public CStrikeModel(Game driver)
        {
            DriverInstance = driver;

            CurState = State.Menu;

            // Initialize UIManager
            InterfaceManager = new UIManager();

            // Initialize buttons
            InterfaceManager.AddComponent(new Button("TestButton", new Rectangle(20, 500, 200, 40), Color.Black, Color.Gray, Color.Blue, "Test", 1.0f, EasingFunctions.AnimationType.QuinticIn));
            
            InterfaceManager.Show("TestButton");
            Input = new InputManager();

            camera = new Camera2D();
            camera.Position = new Vector2((newMap.TileMap.GetLength(0) / 2) * 64 + 32,
                (newMap.TileMap.GetLength(1) / 2) * 64 + 32);
        }

        public void Update(float gameTime)
        {
            Input.Tick();

            InterfaceManager.Update(gameTime);

            switch (CurState)
            {
                case State.Menu:

                    if (fadeIn)
                    {
                        glowTimer += gameTime;

                        if (glowTimer >= 1.0f)
                        {
                            fadeIn = !fadeIn;
                        }
                    }
                    else
                    {
                        glowTimer -= gameTime;

                        if (glowTimer <= 1.0f)
                        {
                            fadeIn = !fadeIn;
                        }
                    }

                    if (Input.Held(Keys.A))
                    {
                        camera.Position.X -= 5f;
                    }

                    if (Input.Held(Keys.D))
                    {
                        camera.Position.X += 5f;
                    }

                    if (Input.Held(Keys.W))
                    {
                        camera.Position.Y -= 5f;
                    }

                    if (Input.Held(Keys.S))
                    {
                        camera.Position.Y += 5f;
                    }

                    stopWatch.Start();
                    collidables = GetCollidableTiles(newMap.ToTile(camera.Col(), camera.Row()));
                    stopWatch.Stop();

                    raycastCollidables = GetWalls(newMap.ToTile(camera.Col(), camera.Row()), (24 * (glowTimer / 1000f)) * (float)Math.PI / 12f);

                    pathfindingTime = Math.Round((double)stopWatch.ElapsedMilliseconds, 10);

                    stopWatch.Reset();

                    break;
                case State.Options:
                    break;
                case State.Lobby:
                    break;
                case State.InGame:
                    break;
            }

            Input.Tock();
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