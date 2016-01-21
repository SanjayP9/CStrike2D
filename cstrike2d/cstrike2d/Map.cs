// Author: Shawn Verma, Mark Voong, Sanjay Paraboo
// File Name: LevelEditor.cs
// Project Name: Global Offensive
// Creation Date: Nov 23rd, 2015
// Modified Date: Jan 19th, 2016
// Description: Stores the map data including tile types and what texture to draw for the client
using System;
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

        public Rectangle MapArea { get; private set; }

        public const int TILE_SIZE = 32;

        private Assets assets;

        public bool Loaded { get; private set; }

        public void Load(string mapName, Assets assets)
        {
            this.assets = assets;

            if (!File.Exists(mapName))
            {
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
            MapArea = new Rectangle(0, 0, TILE_SIZE*MaxCol, TILE_SIZE*MaxRow);

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
                        TileMap[cols, rows] =
                            new Tile((byte) Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 1)),
                                (byte) Convert.ToInt32(rowData[cols].Substring(rowData[cols].Length - 1, 1)),
                                cols, rows, MapArea);
                    }
                }
            }

            // Close the file
            inFile.Close();
            Loaded = true;
        }

        public void Draw(SpriteBatch sb)
        {

            foreach (Tile tile in TileMap)
            {
                if (tile != null)
                {
                    tile.Draw(sb, assets.TileSet);
                }
            }

            /*
            Rectangle destRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
            Rectangle srcRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

            // Draws all tiles placed and the properties highlighted overthem
            for (int col = 0; col < MaxCol; col++)
            {
                for (int row = 0; row < MaxRow; row++)
                {
                    if (TileMap[col, row] != null)
                    {
                        srcRect.X = (TileMap[col, row].TileType % 8 * TILE_SIZE);
                        srcRect.Y = (TileMap[col, row].TileType / 8 * TILE_SIZE);
                        destRect.X = col * TILE_SIZE + MapArea.X;
                        destRect.Y = row * TILE_SIZE + MapArea.Y;

                        sb.Draw(assets.TileSet, destRect, srcRect, Color.White);
                    }
                }
            }
            */
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
