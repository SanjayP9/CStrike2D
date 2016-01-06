using Microsoft.Xna.Framework;

namespace CStrike2D
{
    public class Tile
    {
        public int TileType { get; private set; }
        public Rectangle Bounds { get; private set; }
        
        public Tile(int tileType)
        {
            TileType = tileType;
        }
    }
}
