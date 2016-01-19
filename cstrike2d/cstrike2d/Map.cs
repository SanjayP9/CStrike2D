﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Map
    {
        
        public Tile[,] TileMap { get; private set; }
        public int MaxTiles { get; private set; }
        public int MaxRow { get; private set; }
        public int MaxCol { get; private set; }

        private Rectangle mapArea;

        private const int TILE_SIZE = 32;

        private Assets assets;

        public bool Loaded { get; private set; }

        public void Load(string mapName, Assets assets)
        {
            this.assets = assets;

            if (!File.Exists(mapName))
            {
                System.Console.WriteLine("Missing map: " + mapName);
                Loaded = false;
                return;
            }

            // Creates a stream reader instance of the text file
            StreamReader inFile = File.OpenText(mapName);

            // Stores the data for a single line as a time
            string[] rowData;

            // Checks the first and second line of the text to set the number of columns and the number of rows
            MaxCol = Convert.ToInt32(inFile.ReadLine());
            MaxRow = Convert.ToInt32(inFile.ReadLine());

            // Changes the map area according to the number of columns and rows
            mapArea = new Rectangle(0, 0, TILE_SIZE*MaxCol, TILE_SIZE*MaxRow);

            // Initialize the number of tiles to be according the the number of columns and rows
            TileMap = new Tile[MaxCol, MaxRow];

            // Goes through every line in the text past the first two
            for (int rows = 0; rows < MaxRow; rows++)
            {
                // Sets the row data to be split by commas to siginify a new column
                rowData = inFile.ReadLine().Split(',');

                // Goes through every column in the row
                for (int cols = 0; cols < rowData.Length; cols++)
                {
                    // If the data in the column is not blank
                    if (rowData[cols] != "")
                    {
                        // Initialize each property of the tile
                        TileMap[cols, rows] = new Tile((byte)Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 1)), (byte)Convert.ToInt32(rowData[cols].Substring(rowData[cols].Length - 1, 1)));
                    }
                }
            }

            // Close the file
            inFile.Close();
            Loaded = true;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle destRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
            Rectangle srcRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

            // Draws all tiles placed and the properties highlighted overthem
            for (int col = 0; col < MaxCol; col++)
            {
                for (int row = 0; row < MaxRow; row++)
                {
                    if (TileMap[col, row] != null)
                    {
                        sb.Draw(assets.TileSet, destRect, srcRect, Color.White);
                    }
                }
            }
        }
    }

    static class TileFunctions
    {
        public static int ToTile(int col, int row, int maxCol)
        {
            return (row * maxCol) + col;
        }

        public static int ToTile(float col, float row, int maxCol)
        {
            return (int)((row * maxCol) + col);
        }

        public static int[] FromTile(int tileNumber, int maxCol)
        {
            return new[] { tileNumber % maxCol, tileNumber / maxCol };
        }
    }
}
