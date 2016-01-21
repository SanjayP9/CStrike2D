// Author: Mark Voong
// File Name: ServerMap.cs
// Project Name: Global Offensive
// Creation Date: Jan 19th, 2016
// Modified Date: Jan 19th, 2016
// Description: Serverside version of the Map.cs class on the client.
//              Handles logic in a similar way but used by the server for
//              logic processing
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    class ServerMap
    {
        public CStrike2D.Tile[,] TileMap { get; private set; }

        /// <summary>
        /// List of tiles that are Counter-Terrorist spawn points
        /// </summary>
        public List<CStrike2D.Tile> CTTile { get; private set; }

        /// <summary>
        /// List of tiles that are Terrorist spawn points
        /// </summary>
        public List<CStrike2D.Tile> TTile { get; private set; }

        /// <summary>
        /// Maximum row
        /// </summary>
        public int MaxRow { get; private set; }

        /// <summary>
        /// Maximum column
        /// </summary>
        public int MaxCol { get; private set; }

        /// <summary>
        /// Area of the map
        /// </summary>
        public Rectangle MapArea { get; private set; }

        /// <summary>
        /// Tile size
        /// </summary>
        public const int TILE_SIZE = 32;

        /// <summary>
        /// Loads a map
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public bool Load(string mapName)
        {
            // Intialize lists for CT and T spawn points
            CTTile = new List<CStrike2D.Tile>();
            TTile = new List<CStrike2D.Tile>();

            // Check if the map exists
            if (!File.Exists(mapName))
            {
                Console.WriteLine("Missing map: " + mapName + ". Server cannot start.");
                return false;
            }

            // Creates a instance of file stream, g zip stream and stream reader
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
                CStrike2D.Tile.TILE_SIZE * MaxCol,
                CStrike2D.Tile.TILE_SIZE * MaxRow);

            // Initialize the number of tiles to be according the the number of columns and rows
            TileMap = new CStrike2D.Tile[MaxCol, MaxRow];

            CStrike2D.Tile tile;
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
                        // Initialize the tile
                        tile = new CStrike2D.Tile((byte)Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 1)),
                                (byte)Convert.ToInt32(rowData[cols].Substring(rowData[cols].Length - 1, 1)),
                                cols, rows, MapArea);

                        // Add the tile to the spawn list if it is a spawn tile
                        if (tile.Property == CStrike2D.Tile.CT_SPAWN_POINT)
                        {
                            CTTile.Add(tile);
                        }
                        else if (tile.Property == CStrike2D.Tile.T_SPAWN_POINT)
                        {
                            TTile.Add(tile);
                        }

                        TileMap[cols, rows] = tile;
                    }
                }
            }

            // Close the file
            inFile.Close();

            Console.WriteLine("Map: " + mapName + " successfully loaded");
            return true;
        }
    }
}
