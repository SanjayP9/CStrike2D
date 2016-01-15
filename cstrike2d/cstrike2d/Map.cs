using System;
using System.IO;

namespace CStrike2D
{
    public class Map
    {
        public Tile[,] TileMap { get; private set; }
        public int MaxTiles { get; private set; }
        public int MaxRow { get; private set; }
        public int MaxCol { get; private set; }

        private byte[,] tiles;
        private byte[,] tileProperties;
        int tileSize;
        /*
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

        public void SaveFile(string fileName)
        {
            StreamWriter outFile = File.CreateText(fileName);

            //in the homework he had tile size we prob will max tile size a constant
            outFile.WriteLine(tileSize);

            outFile.WriteLine(MaxRow);
            outFile.WriteLine(MaxCol);

            for (int rows = 0; rows < MaxRow; rows++)
            {
                for (int cols = 0; cols < MaxCol; cols++)
                {
                    outFile.Write(tiles[rows, cols] + ",");
                }
                outFile.WriteLine();
            }

            for (int rows = 0; rows < MaxRow; rows++)
            {
                for (int cols = 0; cols < MaxCol; cols++)
                {
                    outFile.Write(tileProperties[rows, cols] + ",");
                }
                outFile.WriteLine();
            }

            outFile.Close();
        }
        
        public void LoadFile(string fileName)
        {
            StreamReader inFile = File.OpenText(fileName);

            string[] rowData;

            tileSize = Convert.ToInt32(inFile.ReadLine());
            MaxRow = Convert.ToInt32(inFile.ReadLine());
            MaxCol = Convert.ToInt32(inFile.ReadLine());

            tiles = new byte[MaxRow, MaxCol];
            tileProperties = new byte[MaxRow, MaxCol];

            for (int rows = 0; rows < MaxRow; rows++)
            {
                rowData = inFile.ReadLine().Split(',');

                for (int cols = 0; cols < MaxCol; cols++)
                {
                    tiles[rows, cols] = Convert.ToByte(rowData[cols]);
                }
            }

            for (int rows = 0; rows < MaxRow; rows++)
            {

                rowData = inFile.ReadLine().Split(',');

                for (int cols = 0; cols < MaxCol; cols++)
                {
                    tileProperties[rows, cols] = Convert.ToByte(rowData[cols]);
                }
            }

            inFile.Close();
        }

        /// <summary>
        /// Returns the tile type given the tile number
        /// </summary>
        /// <param name="tileNumber"></param>
        /// <returns></returns>
        public int TypeFromTileNumber(int tileNumber)
        {
            int[] rowCol = { tileNumber % MaxCol, tileNumber / MaxCol };

            return TileMap[rowCol[0], rowCol[1]].TileType;
        }

        /// <summary>
        /// Returns the coordinate of a tile in [col, row]
        /// </summary>
        /// <param name="tileNumber"></param>
        /// <returns></returns>
        public int[] FromTile(int tileNumber)
        {
            return new[] { tileNumber % MaxCol, tileNumber / MaxCol };
        }

        /// <summary>
        /// Gives the tile number given the column and row
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public int ToTile(int col, int row)
        {
            return (row * MaxCol) + col;
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
