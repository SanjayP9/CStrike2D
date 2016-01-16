using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class LevelEditor
    {
        Tile[,] tiles;
        int selectedTile;
        float tileSize = 32;

        int numRows = 10;
        int numCols = 8;

        private InputManager input;
        private AudioManager audio;
        private CStrike2D driver;

        Rectangle tileSetOffset = new Rectangle(50, 50, 256, 320);
        Rectangle placementArea = new Rectangle(300, 300, 256, 320);
        Vector2 SelectedTileLoc;

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            audio = model.AudioManager;
        }

        public void Update(float gameTime)
        {
            if (input.LeftClick() == true)
            {
                if (input.MousePosition.X > tileSetOffset.X &&
                   input.MousePosition.X < tileSetOffset.X + tileSetOffset.Width &&
                   input.MousePosition.Y > tileSetOffset.Y &&
                   input.MousePosition.Y < tileSetOffset.Y + tileSetOffset.Height)
                {
                    //SelectedTileLoc = new Vector2((float)((int)(input.MousePosition.X - tileSetOffset.X)), (float)((int)(input.MousePosition.Y - tileSetOffset.Y)));
                    selectedTile = (int)((input.MousePosition.Y - tileSetOffset.Y) / tileSize) * 8 +
                                   (int)((input.MousePosition.X - tileSetOffset.X) / tileSize);
                }

                if (input.MousePosition.X > placementArea.X &&
                    input.MousePosition.X < placementArea.X + placementArea.Width &&
                    input.MousePosition.Y > placementArea.Y &&
                    input.MousePosition.Y < placementArea.Y + placementArea.Height)
                {
                    Vector2 placedTileLocation = new Vector2(((input.MousePosition.X - tileSetOffset.X) / tileSize), (input.MousePosition.Y - tileSetOffset.Y) / tileSize);
                    tiles[(int)((input.MousePosition.X - tileSetOffset.X) / tileSize), 
                          (int)((input.MousePosition.Y - tileSetOffset.Y) / tileSize)] = new Tile(selectedTile);
                }
            }
        }

        public void DrawWorld(SpriteBatch sb)
        {
            for (int x = 0; x < numRows; x++)
            {
                for (int y = 0; y < numCols; y++)
                {
                    int srcX = (int)(tiles[x, y].TileType % 8 * tileSize);
                    int srcY = (int)(tiles[x, y].TileType / 8 * tileSize);

                    Rectangle tileSrcRec = new Rectangle(srcX, srcY, (int)tileSize, (int)tileSize);
                    //if (tiles[x, y])
                    //{
                        sb.Draw(driver.Assets.TileSet, new Rectangle((int)(x * tileSize + placementArea.X), (int)(y * tileSize + placementArea.Y), (int)tileSize, (int)tileSize), tileSrcRec, Color.White);
                    //}
                }
            }
        }

        public void DrawUI(SpriteBatch sb)
        {
            sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);
            sb.DrawString(driver.Assets.DefaultFont, "" + selectedTile, new Vector2 (0,0), Color.White );

            //sb.DrawString(driver.Assets.DefaultFont, "X", new Vector2(SelectedTileLoc.X, SelectedTileLoc.Y), Color.White);
        }
    }
}
