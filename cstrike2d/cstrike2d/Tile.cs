// Author: Mark Voong, Shawn Verma, Sanjay Paraboo
// File Name: Tile.cs
// Project Name: Global Offensive
// Creation Date: Nov 23rd, 2015
// Modified Date: Jan 19th, 2016
// Description: Holds all of the data a tile including its type,
//              and various properties such as being a spawnpoint or is
//              collidable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Tile
    {
        public Rectangle TileRect
        {
            get
            {
                return destRect;
            }

            private set
            {
                destRect = value;
            }
        }

        private Rectangle destRect;
        private Rectangle srcRect;
        private Color tileColor;

        public const int TILE_SIZE = 32;

        public Rectangle Bounds { get; private set; }
        public byte Property { get; private set; }

        public const byte NO_PROPERTY = 0;
        public const byte SOLID = 1;
        public const byte A_PLANT_SPOT = 2;
        public const byte B_PLANT_SPOT = 3;
        public const byte SAVE_SPOT = 4;
        public const byte CT_SPAWN_POINT = 5;
        public const byte T_SPAWN_POINT = 6;
        public const byte A_SITE_DEFENCE_POINT = 7;
        public const byte B_SITE_DEFENCE_POINT = 8;

        public int FCost { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public byte TileType { get; private set; }

        public Tile(byte tileType, byte property, int col, int row, Rectangle mapArea)
        {
            TileType = tileType;
            Property = property;
            destRect.X = col * TILE_SIZE + mapArea.X;
            destRect.Y = row * TILE_SIZE + mapArea.Y;
            destRect.Width = TILE_SIZE;
            destRect.Height = TILE_SIZE;
            srcRect.X = (TileType % 8 * TILE_SIZE);
            srcRect.Y = (TileType / 8 * TILE_SIZE);
            srcRect.Width = TILE_SIZE;
            srcRect.Height = TILE_SIZE;
        }
        public void SetProperty(byte property)
        {
            Property = property;
        }

        public void SetTileType(byte tileType)
        {
            TileType = tileType;
            srcRect.X = (TileType % 8 * TILE_SIZE);
            srcRect.Y = (TileType / 8 * TILE_SIZE);
        }

        public void Draw(SpriteBatch sb, Texture2D tileSet, bool editorMode = false)
        {

            if (editorMode)
            {
                switch (Property)
                {
                    case NO_PROPERTY:
                        tileColor = Color.White;
                        break;
                    case SOLID:
                        tileColor = Color.Red;
                        break;
                    case A_PLANT_SPOT:
                        tileColor = Color.Yellow;
                        break;
                    case B_PLANT_SPOT:
                        tileColor = Color.Yellow;
                        break;
                    case SAVE_SPOT:
                        tileColor = Color.Green;
                        break;
                    case CT_SPAWN_POINT:
                        tileColor = Color.Blue;
                        break;
                    case T_SPAWN_POINT:
                        tileColor = Color.Orange;
                        break;
                    case A_SITE_DEFENCE_POINT:
                        tileColor = Color.Purple;
                        break;
                    case B_SITE_DEFENCE_POINT:
                        tileColor = Color.Purple;
                        break;
                }
            }
            else
            {
                tileColor = Color.White;
            }

            sb.Draw(tileSet, destRect, srcRect, tileColor);
        }
    }
}
