using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Map
    {
        /*
        public Tile[,] TileMap { get; private set; }
        public int MaxTiles { get; private set; }
        public int MaxRow { get; private set; }
        public int MaxCol { get; private set; }

        public Vector2 MapArea { get; private set; }

        private const int TILE_SIZE = 32;

        private Assets assets;

        public void Load(string mapName, Assets assets)
        {
            /*
            this.assets = assets;

            if (!File.Exists(mapName))
            {
                System.Console.WriteLine("Missing map: " + mapName);
                return;
            }

            StreamReader inFile = File.OpenText(mapName);

            // Stores the data for a single line at a time

            // Checks the first and second line of the text to set the number of columns and the number of rows
            MaxCol = Convert.ToInt32(inFile.ReadLine());
            MaxRow = Convert.ToInt32(inFile.ReadLine());

            // Changes the placement area according to the number of columns and rows
            MapArea = new Rectangle(-200, -250, 32 * MaxCol, 32 * MaxRow);

            // Initialize the number of tiles to be according the the number of columns and rows
            Tiles = new Tile[MaxCol, MaxRow];

            // Goes through every line in the text past the first two
            for (int rows = 0; rows < MaxRow; rows++)
            {
                // Sets the row data to be split by commas to siginify a new column
                string[] rowData = inFile.ReadLine().Split(',');

                // Goes through every column in the row
                for (int cols = 0; cols < rowData.Length; cols++)
                {
                    // If the data in the column is not blank
                    if (rowData[cols] != "")
                    {
                        // According to each character in the text check to see for the 0/1
                        // for each property and set the property to be true or false accordingly
                        bool isPlantSpot = rowData[cols][rowData[cols].Length - 6] == '1';

                        bool isSaveSpot = rowData[cols][rowData[cols].Length - 5] == '1';

                        bool isSolid = rowData[cols][rowData[cols].Length - 4] == '1';

                        bool isCTSpawnPoint = rowData[cols][rowData[cols].Length - 3] == '1';

                        bool isTSpawnPoint = rowData[cols][rowData[cols].Length - 2] == '1';

                        bool isSiteDefencePoint = rowData[cols][rowData[cols].Length - 1] == '1';

                        // Initialize each property of the tile
                        Tiles[cols, rows] = new Tile(Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 6)), isPlantSpot, isSaveSpot, isSolid, isCTSpawnPoint, isTSpawnPoint, isSiteDefencePoint);
                    }
                }
            }

            // Close the file
            inFile.Close();

            
        }


        public void Draw(SpriteBatch sb, Assets assets)
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
                        srcRect.X = (TileMap[col, row].TileType % 8 * TILE_SIZE);
                        srcRect.Y = (TileMap[col, row].TileType / 8 * TILE_SIZE);
                        destRect.X = col * TILE_SIZE + MapArea.X;
                        destRect.Y = row * TILE_SIZE + MapArea.Y;
                        sb.Draw(assets.TileSet, destRect, srcRect, Color.White);
                    }
                }
            }
        }

        
        public Map(int[,] tileData)
        {
            TileMap = new Tile[tileData.GetLength(0), tileData.GetLength(1)];

            for (int y = 0; y < tileData.GetLength(1); y++)
            {
                for (int x = 0; x < tileData.GetLength(0); x++)
                {
                    TileMap[x, y] = new Tile(tileData[x, y]);
                }
            }

            MaxTiles = tileData.GetLength(0) * tileData.GetLength(1);
            MaxRow = tileData.GetLength(1);
            MaxCol = tileData.GetLength(0);
        }


        */

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
