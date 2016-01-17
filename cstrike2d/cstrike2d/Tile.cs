using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Tile
    {
        const int BORDER_THICKNESS = 1;
        public Rectangle TileRect { get; private set; }
        public int TileType { get; private set; }
        public Rectangle Bounds { get; private set; }
        public bool IsSolid { get; set; }
        public bool IsPlantSpot { get; set; }
        public bool IsSaveSpot { get; set; }

        public Tile(int tileType, bool isPlantSpot, bool isSaveSpot, bool isSolid)
        {
            TileType = tileType;
            IsPlantSpot = isPlantSpot;
            IsSaveSpot = isSaveSpot;
            IsSolid = isSolid;
        }

        public void Draw(SpriteBatch sb, Texture2D tileTexture)
        {
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, TileRect.Width, TileRect.Height), Color.White);

            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, BORDER_THICKNESS, TileRect.Height), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, TileRect.Width, BORDER_THICKNESS), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Bottom - BORDER_THICKNESS, TileRect.Width, BORDER_THICKNESS), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.Right - BORDER_THICKNESS, TileRect.Y, BORDER_THICKNESS, TileRect.Height), Color.Black);
        }
    }
}
