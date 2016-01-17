using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace CStrike2D
{
    public class LevelEditor
    {
        Tile[,] tiles = new Tile[50, 50];
        int selectedTile;
        float tileSize = 32;

        int numRows = 50;
        int numCols = 50;

        Vector2 mouseWorld;
        Vector2 placedTilePos;
        bool isPlantSpot;
        bool isSaveSpot;
        bool isSolid;

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
            //LoadFile("world1.txt");
            mouseWorld = input.ScreenToWorld(input.MousePosition, driver.Model.Camera, driver.Model.Center);
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
                if (mouseWorld.X > placementArea.X &&
                         mouseWorld.X < placementArea.X + placementArea.Width &&
                         mouseWorld.Y > placementArea.Y &&
                         mouseWorld.Y < placementArea.Y + placementArea.Height)
                {
                    placedTilePos = new Vector2(((mouseWorld.X - placementArea.X) / tileSize), (mouseWorld.Y - placementArea.Y) / tileSize);   
                    tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = new Tile(selectedTile, false, false, false);
                }
            }
            
            if (mouseWorld.X > placementArea.X &&
                            mouseWorld.X < placementArea.X + placementArea.Width &&
                            mouseWorld.Y > placementArea.Y &&
                            mouseWorld.Y < placementArea.Y + placementArea.Height)
            {
                placedTilePos = new Vector2(((mouseWorld.X - placementArea.X) / tileSize), (mouseWorld.Y - placementArea.Y) / tileSize);
                if (input.LeftHold())
                {
                    tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = new Tile(selectedTile, false, false, false);
                }
                else if (input.RightHold())
                {
                    tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = null;
                }
                if (input.Tapped(Keys.Z))
                {
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsPlantSpot = !tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsPlantSpot;
                    }
                }
                else if (input.Tapped(Keys.X))
                {
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSaveSpot = !tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSaveSpot;
                    }
                }
                else if (input.Tapped(Keys.C))
                {
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSolid = !tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSolid;
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
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.Red * 0.5f);
                        }
                        if (tiles[x, y].IsSaveSpot)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.Yellow * 0.5f);
                        }
                        if (tiles[x, y].IsSolid)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.Blue * 0.5f);
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
            for (int x = 0; x <= 8; x++)
            {
                sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(tileSetOffset.X + x * tileSize), (int)tileSetOffset.Y, 1, (int)tileSetOffset.Height), Color.Black);
            }
            for (int y = 0; y <= 10; y++)
            {
                sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)tileSetOffset.X, (int)(tileSetOffset.Y + y * tileSize), (int)tileSetOffset.Width, 1), Color.Black);
            }
        }
        public void LoadFile(string fileName)
        {
            StreamReader inFile = File.OpenText(fileName);
            int lineCount = File.ReadLines(fileName).Count() - 2;
            string[] rowData;

            numCols = Convert.ToInt32(inFile.ReadLine());
            numRows = Convert.ToInt32(inFile.ReadLine());

            tiles = new Tile[numCols, numRows];

            for (int rows = 0; rows < lineCount; rows++)
            {
                rowData = inFile.ReadLine().Split(',');

                for (int cols = 0; cols < rowData.Length; cols++)
                {
                    if (rowData[cols] != "")
                    {
                        if (rowData[cols][rowData[cols].Length - 3] == '1')
                        {
                            isPlantSpot = true;
                        }
                        else
                        {
                            isPlantSpot = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 2] == '1')
                        {
                            isSaveSpot = true;
                        }
                        else
                        {
                            isSaveSpot = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 1] == '1')
                        {
                            isSolid = true;
                        }
                        else
                        {
                            isSolid = false;
                        }

                        tiles[cols, rows] = new Tile(Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 4)), isSaveSpot, isPlantSpot, isSolid);
                    }
                }
            }
            inFile.Close();
        }
        public void SaveFile(string fileName)
        {
            StreamWriter outFile = File.CreateText(fileName);

            outFile.WriteLine(numCols);
            outFile.WriteLine(numRows);

            for (int rows = 0; rows < numRows; rows++)
            {
                for (int cols = 0; cols < numCols; cols++)
                {
                    string isPlantSpotString;
                    string isSaveSpotString;
                    string isSolidString;
                    if (tiles[cols, rows] != null)
                    {
                        if (tiles[cols, rows].IsPlantSpot)
                        {
                            isPlantSpotString = "1";
                        }
                        else
                        {
                            isPlantSpotString = "0";
                        }
                        if (tiles[cols, rows].IsSaveSpot)
                        {
                            isSaveSpotString = "1";
                        }
                        else
                        {
                            isSaveSpotString = "0";
                        }
                        if (tiles[cols, rows].IsSolid)
                        {
                            isSolidString = "1";
                        }
                        else
                        {
                            isSolidString = "0";
                        }
                        outFile.Write(Convert.ToString(tiles[cols, rows].TileType) + isPlantSpotString + isSaveSpotString + isSolidString + ",");
                    }
                    outFile.Write(",");
                }
                outFile.WriteLine();
            }
            outFile.Close();
        }
    }
}
