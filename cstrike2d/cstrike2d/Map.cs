
using System.Windows.Forms.VisualStyles;

namespace CStrike2D
{
    public class Map
    {
        public Tile[,] TileMap { get; private set; }
        public int MaxTiles { get; private set; }
        public int MaxRow { get; private set; }
        public int MaxCol { get; private set; }

        public Map(int[,] tileData)
        {
            TileMap = new Tile[tileData.GetLength(0), tileData.GetLength(1)];

            for (int y = 0; y < tileData.GetLength(1); y++)
            {
                for (int x = 0; x < tileData.GetLength(0); x++)
                {
                    TileMap[x, y] = new Tile(tileData[x, y], false, false);
                }
            }

            MaxTiles = tileData.GetLength(0)*tileData.GetLength(1);
            MaxRow = tileData.GetLength(1);
            MaxCol = tileData.GetLength(0);
        }

        /// <summary>
        /// Returns the tile type given the tile number
        /// </summary>
        /// <param name="tileNumber"></param>
        /// <returns></returns>
        public int TypeFromTileNumber(int tileNumber)
        {
            int[] rowCol = { tileNumber % MaxCol, tileNumber / MaxCol};

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
