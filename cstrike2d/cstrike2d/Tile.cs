// Author: Mark Voong, Shawn Verma, Sanjay Paraboo
// File Name: Tile.cs
// Project Name: Global Offensive
// Creation Date: Nov 23rd, 2015
// Modified Date: Jan 21st, 2016
// Description: Holds all of the data a tile including its type,
//              and various properties such as being a spawnpoint or is
//              collidable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Tile
    {
        /// <summary>
        /// Dimensions of a tile
        /// </summary>
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

        private Rectangle destRect;     // Location of a tile on the map
        private Rectangle srcRect;      // What part of the tileset to draw from
        private Color tileColor;        // Colour of the tile

        public const int TILE_SIZE = 32;    // Size of a tile

        /// <summary>
        /// The property of a tile
        /// </summary>
        public byte Property { get; private set; }

        // Differernt property types
        public const byte NO_PROPERTY = 0;          // None
        public const byte SOLID = 1;                // Collidable
        public const byte A_PLANT_SPOT = 2;         // A-Site plant spot
        public const byte B_PLANT_SPOT = 3;         // B-Site plant spot
        public const byte SAVE_SPOT = 4;            // AI Node for saving weapons for next round
        public const byte CT_SPAWN_POINT = 5;       // CT Spawn point
        public const byte T_SPAWN_POINT = 6;        // T Spawn point
        public const byte A_SITE_DEFENCE_POINT = 7; // AI Node for them to protect the bomb
        public const byte B_SITE_DEFENCE_POINT = 8; // AI Node for them to protect the bomb

        /// <summary>
        /// F cost for AI pathfinding
        /// </summary>
        public int FCost { get; set; }

        /// <summary>
        /// G cost for AI pathfinding
        /// </summary>
        public int GCost { get; set; }

        /// <summary>
        /// H cost for AI pathfinding
        /// </summary>
        public int HCost { get; set; }
        
        /// <summary>
        /// Type of tile
        /// </summary>
        public byte TileType { get; private set; }

        /// <summary>
        /// Creates a new tile
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="property"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="mapArea"></param>
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

        /// <summary>
        /// Changes the property of a tile
        /// </summary>
        /// <param name="property"></param>
        public void SetProperty(byte property)
        {
            Property = property;
        }

        /// <summary>
        /// Changes the type of tile
        /// </summary>
        /// <param name="tileType"></param>
        public void SetTileType(byte tileType)
        {
            TileType = tileType;
            srcRect.X = (TileType % 8 * TILE_SIZE);
            srcRect.Y = (TileType / 8 * TILE_SIZE);
        }

        /// <summary>
        /// Draws the tile
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="tileSet"></param>
        /// <param name="editorMode"></param>
        public void Draw(SpriteBatch sb, Texture2D tileSet, bool editorMode = false)
        {

            // If in editor mode, draw a colour overlay
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

            // Draw the tile
            sb.Draw(tileSet, destRect, srcRect, tileColor);
        }
    }
}
