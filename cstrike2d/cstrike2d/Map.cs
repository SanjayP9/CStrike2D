// Author: Shawn Verma, Mark Voong, Sanjay Paraboo
// File Name: LevelEditor.cs
// Project Name: Global Offensive
// Creation Date: Nov 23rd, 2015
// Modified Date: Jan 19th, 2016
// Description: Stores the map data including tile types and what texture to draw for the client
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Map
    {
        /// <summary>
        /// Stores the tile data
        /// </summary>
        public Tile[,] TileMap { get; private set; }

        /// <summary>
        /// The maximum row
        /// </summary>
        public int MaxRow { get; private set; }

        /// <summary>
        /// The maximum column
        /// </summary>
        public int MaxCol { get; private set; }

        /// <summary>
        /// The rectangle dimensions of the map
        /// </summary>
        public Rectangle MapArea { get; private set; }

        /// <summary>
        /// Width/Height of one tile
        /// </summary>
        public const int TILE_SIZE = 32;       

        // Assets
        private Assets assets;

        /// <summary>
        /// Has the map been loaded
        /// </summary>
        public bool Loaded { get; private set; }

        public Tile[] Solids { get; private set; } 

        /// <summary>
        /// Loads the map
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="assets"></param>
        public void Load(string mapName, Assets assets)
        {
            this.assets = assets;
            List<Tile> solids = new List<Tile>();

            // Check if the map exists before trying to load it
            if (!File.Exists(mapName))
            {
                Loaded = false;
                return;
            }

            // File compression
            FileStream decompressedFile = new FileStream(mapName, FileMode.Open, FileAccess.Read);
            GZipStream gZip = new GZipStream(decompressedFile, CompressionMode.Decompress);
            StreamReader inFile = new StreamReader(gZip);

            // Stores the data for a single line as a time
            string[] rowData;

            // Reads the author name and description
            inFile.ReadLine();
            inFile.ReadLine();

            // Reads the number of columns and the number of rows
            MaxCol = Convert.ToInt32(inFile.ReadLine());
            MaxRow = Convert.ToInt32(inFile.ReadLine());

            // Changes the map area according to the number of columns and rows
            MapArea = new Rectangle(0, 0,
                Tile.TILE_SIZE * MaxCol,
                Tile.TILE_SIZE * MaxRow);

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
                            new Tile((byte)Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 1)),
                                (byte)Convert.ToInt32(rowData[cols].Substring(rowData[cols].Length - 1, 1)),
                                cols, rows, MapArea);

                        if (TileMap[cols, rows].Property == Tile.SOLID)
                        {
                            solids.Add(TileMap[cols,rows]);
                        }
                    }
                }
            }

            // Save the list of solids to the public array
            Solids = solids.ToArray();

            // Close the file
            inFile.Close();
            Loaded = true;
        }

        /// <summary>
        /// Draws the tiles
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {

            foreach (Tile tile in TileMap)
            {
                if (tile != null)
                {
                    tile.Draw(sb, assets.TileSet);
                }
            }
        }
    }
}
