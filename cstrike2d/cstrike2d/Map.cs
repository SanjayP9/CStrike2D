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
