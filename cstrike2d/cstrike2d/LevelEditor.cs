using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class LevelEditor
    {
        Tile[,] tiles = new Tile[50, 50];
        int selectedTile;
        float tileSize = 32;

        int numRows = 50;
        int numCols = 50;

        Vector2 placedTilePos;
        bool isPlantSpot;
        bool isSaveSpot;

        private InputManager input;
        private AudioManager audio;
        private CStrike2D driver;

        Rectangle tileSetOffset = new Rectangle(50, 100, 256, 320);
        Rectangle placementArea = new Rectangle(-200, -250, 1600, 1600);
        Vector2 SelectedTileLoc;

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            audio = model.AudioManager;
        }

        public void Update(float gameTime)
        {
            Vector2 mouseWorld = input.ScreenToWorld(input.MousePosition, driver.Model.Camera, driver.Model.Center);
            if (input.LeftClick() == true)
            {
                if (input.MousePosition.X > tileSetOffset.X &&
                    input.MousePosition.X < tileSetOffset.X + tileSetOffset.Width &&
                    input.MousePosition.Y > tileSetOffset.Y &&
                    input.MousePosition.Y < tileSetOffset.Y + tileSetOffset.Height)
                {
                    selectedTile = (int)((input.MousePosition.Y - tileSetOffset.Y) / tileSize) * 8 +
                                   (int)((input.MousePosition.X - tileSetOffset.X) / tileSize);
                }
                else if (mouseWorld.X > placementArea.X &&
                         mouseWorld.X < placementArea.X + placementArea.Width &&
                         mouseWorld.Y > placementArea.Y &&
                         mouseWorld.Y < placementArea.Y + placementArea.Height)
                {
                    placedTilePos = new Vector2(((mouseWorld.X - placementArea.X) / tileSize), (mouseWorld.Y - placementArea.Y) / tileSize);
                    if((selectedTile != 0) || (selectedTile != 72))
                    {
                        tiles[(int)((mouseWorld.X - placementArea.X) / tileSize),
                              (int)((mouseWorld.Y - placementArea.Y) / tileSize)] = new Tile(selectedTile, false, false);
                    }
                    else if(selectedTile == 0 && tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsPlantSpot = true;
                    }
                    else if (selectedTile == 72 && tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSaveSpot = true;
                    }
                }
            }

            if (input.Held(Keys.W))
            {
                driver.Model.Camera.Position.Y -= 5;
            }
            else if (input.Held(Keys.S))
            {
                driver.Model.Camera.Position.Y += 5;
            }
            if (input.Held(Keys.A))
            {
                driver.Model.Camera.Position.X -= 5;
            }
            else if (input.Held(Keys.D))
            {
                driver.Model.Camera.Position.X += 5;
            }
        }

        public void DrawWorld(SpriteBatch sb)
        {
            sb.Draw(driver.Assets.PixelTexture, placementArea, Color.CornflowerBlue);
            for (int x = 0; x < numRows; x++)
            {
                for (int y = 0; y < numCols; y++)
                {
                    if (tiles[x, y] != null)
                    {
                        int srcX = (int)(tiles[x, y].TileType % 8 * tileSize);
                        int srcY = (int)(tiles[x, y].TileType / 8 * tileSize);
                        Rectangle tileSrcRec = new Rectangle(srcX, srcY, (int)tileSize, (int)tileSize);

                        sb.Draw(driver.Assets.TileSet, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     tileSrcRec,
                                                                     Color.White);

                        if(tiles[x, y].IsPlantSpot)
                        {
                            sb.Draw(driver.Assets.TileSet, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.White * 0.5f);
                        }
                    }
                }
            }
            for (int x = 0; x <= numRows; x++)
            {
                sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(placementArea.X + x * tileSize), (int)placementArea.Y, 1, (int)placementArea.Height), Color.Black);
            }
            for (int y = 0; y <= numCols; y++)
            {
                sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)placementArea.X, (int)(placementArea.Y + y * tileSize), (int)placementArea.Width, 1), Color.Black);
            }
        }

        public void DrawUI(SpriteBatch sb)
        {
            sb.DrawString(driver.Assets.DefaultFont, "" + input.ScreenToWorld(input.MousePosition, driver.Model.Camera, driver.Model.Center), new Vector2(100, 0), Color.White);
            sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);
            sb.DrawString(driver.Assets.DefaultFont, "" + selectedTile, new Vector2 (0,0), Color.White );
        }
    }
}
