using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Tile
    {
        const int BORDER_THICKNESS = 1;
        public Rectangle TileRect { get; private set; }

       // public int TileType { get; private set; }
        public Rectangle Bounds { get; private set; }
        public bool IsSolid { get; private set; }

        private Color tileColor;
        public Tile(/*int tileType,*/ Rectangle tileRect, bool isSolid)
        {
           // TileType = tileType;

            this.TileRect = tileRect;
            this.IsSolid = isSolid;

            switch (IsSolid)
            {
                case true:
                    tileColor = Color.Blue;
                    break;

                case false:
                    tileColor = Color.White;
                    break;
            }
        }

        public void Draw(SpriteBatch sb, Texture2D tileTexture)
        {
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, TileRect.Width, TileRect.Height), tileColor);

            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, BORDER_THICKNESS, TileRect.Height), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Y, TileRect.Width, BORDER_THICKNESS), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.X, TileRect.Bottom - BORDER_THICKNESS, TileRect.Width, BORDER_THICKNESS), Color.Black);
            sb.Draw(tileTexture, new Rectangle(TileRect.Right - BORDER_THICKNESS, TileRect.Y, BORDER_THICKNESS, TileRect.Height), Color.Black);
        }
    }
}
