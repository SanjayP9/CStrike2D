// Author: Mark Voong
// File Name: Tile.cs
// Project Name: Global Offensive
// Creation Date: Jan 19th, 2016
// Modified Date: Jan 19th, 2016
// Description: Serverside version of the Tile.cs class on the client.
//              Handles logic in a similar way but used by the server for
//              logic processing
using Microsoft.Xna.Framework;

namespace CStrike2DServer
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
            destRect.X = col*ServerMap.TILE_SIZE + mapArea.X;
            destRect.Y = row*ServerMap.TILE_SIZE + mapArea.Y;
            destRect.Width = ServerMap.TILE_SIZE;
            destRect.Height = ServerMap.TILE_SIZE;
        }

        public void SetProperty(byte property)
        {
            Property = property;
        }

        public void SetTileType(byte tileType)
        {
            TileType = tileType;
        }
    }
}
