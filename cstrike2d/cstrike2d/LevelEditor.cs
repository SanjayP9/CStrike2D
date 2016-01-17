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
        Tile[,] tiles = new Tile[75, 50];
        int selectedTile;
        float tileSize = 32;
        int NUMBER_OF_TILES_WIDE = 8;
        int NUMBER_OF_TILES_LONG = 10;

        int numRows = 50;
        int numCols = 75;

        Vector2 mouseWorld;
        Vector2 placedTilePos;

        // Stores the properties of a tile
        bool isPlantSpot;
        bool isSaveSpot;
        bool isSolid;
        bool isCTSpawnPoint;
        bool isTSpawnPoint;
        bool isSiteDefencePoint;

        private InputManager input;
        private AudioManager audio;
        private CStrike2D driver;

        Rectangle tileSetOffset = new Rectangle(50, 100, 256, 320);
        Rectangle placementArea = new Rectangle(-200, -250, 2400, 1600);

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            audio = model.AudioManager;
        }

        public void Update(float gameTime)
        {
            // Holds the mouse position according to the world
            mouseWorld = input.ScreenToWorld(input.MousePosition, driver.Model.Camera, driver.Model.Center);
            LoadFile("de_cache.txt");

            // If left mouse is clicked
            if (input.LeftClick())
            {
                // If the mouse if over the tile set
                if (input.MousePosition.X > tileSetOffset.X &&
                    input.MousePosition.X < tileSetOffset.X + tileSetOffset.Width &&
                    input.MousePosition.Y > tileSetOffset.Y &&
                    input.MousePosition.Y < tileSetOffset.Y + tileSetOffset.Height)
                {
                    // Set the selected tile to be the one that they have clicked
                    selectedTile = (int)((input.MousePosition.Y - tileSetOffset.Y) / tileSize) * NUMBER_OF_TILES_WIDE +
                                   (int)((input.MousePosition.X - tileSetOffset.X) / tileSize);
                }
            }
            
            // If the mouse is over the map region
            if (mouseWorld.X > placementArea.X &&
                mouseWorld.X < placementArea.X + placementArea.Width &&
                mouseWorld.Y > placementArea.Y &&
                mouseWorld.Y < placementArea.Y + placementArea.Height)
            {
                // Find the tile the mouse is on
                placedTilePos = new Vector2(((mouseWorld.X - placementArea.X) / tileSize), (mouseWorld.Y - placementArea.Y) / tileSize);

                // If the left mouse button is held
                if (input.LeftHold())
                {
                    // Set the selected tile to be in that tile position according to the mouse keeping the properties if any
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetTileType(selectedTile);
                    }
                    else
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = new Tile(selectedTile, false, false, false, false, false, false);
                    }
                }
                // If the right mouse button is held
                else if (input.RightHold())
                {
                    // Set the tile that the mouse is on to be empty
                    tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = null;
                }

                // If the tile the mouse is on is not null
                if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                {
                    // 
                    if (input.Tapped(Keys.Z))
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsPlantSpot(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsPlantSpot);
                    }
                    // 
                    else if (input.Tapped(Keys.X))
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsSaveSpot(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSaveSpot);
                    }
                    // 
                    else if (input.Tapped(Keys.C))
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsSolid(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSolid);
                    }
                    // 
                    else if (input.Tapped(Keys.V))
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsCTSpawnPoint(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsCTSpawnPoint);
                    }
                    // 
                    else if (input.Tapped(Keys.B))
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsTSpawnPoint(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsTSpawnPoint);
                    }
                    // 
                    else if (input.Tapped(Keys.N))
                    {
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsSiteDefencePoint(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSiteDefencePoint);
                    }
                }
            }

            // Move the camera up when the W key is held
            if (input.Held(Keys.W))
            {
                driver.Model.Camera.Position.Y -= 5;
            }
            // Move the camera down when the S key is held
            else if (input.Held(Keys.S))
            {
                driver.Model.Camera.Position.Y += 5;
            }
            // Move the camera left when the A key is held
            if (input.Held(Keys.A))
            {
                driver.Model.Camera.Position.X -= 5;
            }
            // Move the camera right when the D key is held
            else if (input.Held(Keys.D))
            {
                driver.Model.Camera.Position.X += 5;
            }
            if (input.Tapped(Keys.Enter))
            {
                SaveFile("de_cache.txt");
            }
        }

        public void DrawWorld(SpriteBatch sb)
        {
            sb.Draw(driver.Assets.PixelTexture, placementArea, Color.CornflowerBlue);

            // Draws all tiles placed and the properties highlighted overthem
            for (int x = 0; x < numCols; x++)
            {
                for (int y = 0; y < numRows; y++)
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
                        if (tiles[x, y].IsCTSpawnPoint)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.Brown * 0.5f);
                        }
                        if (tiles[x, y].IsTSpawnPoint)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.Green * 0.5f);
                        }
                        if (tiles[x, y].IsSiteDefencePoint)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(x * tileSize + placementArea.X),
                                                                     (int)(y * tileSize + placementArea.Y),
                                                                     (int)tileSize,
                                                                     (int)tileSize),
                                                                     new Rectangle(0, 0, 32, 32),
                                                                     Color.Purple * 0.5f);
                        }
                    }
                }
            }

            // Draws a grid line for the map each line being 1 pixel thick
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
            sb.DrawString(driver.Assets.DefaultFont, "" + selectedTile, new Vector2(0, 0), Color.White);

            // Draw the tile set
            sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);

            // Drawn the grid line for the tile set each line being 1 pixel thick
            for (int x = 0; x <= NUMBER_OF_TILES_WIDE; x++)
            {
                sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(tileSetOffset.X + x * tileSize), (int)tileSetOffset.Y, 1, (int)tileSetOffset.Height), Color.Black);
            }
            for (int y = 0; y <= NUMBER_OF_TILES_LONG; y++)
            {
                sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)tileSetOffset.X, (int)(tileSetOffset.Y + y * tileSize), (int)tileSetOffset.Width, 1), Color.Black);
            }
        }
        public void LoadFile(string fileName)
        {
            // 
            StreamReader inFile = File.OpenText(fileName);

            // Count the amount of lines in the text file
            int lineCount = File.ReadLines(fileName).Count();

            // Stores the data for a single line as a time
            string[] rowData;

            // Checks the first and second line of the text to set the number of columns and the number of rows
            numCols = Convert.ToInt32(inFile.ReadLine());
            numRows = Convert.ToInt32(inFile.ReadLine());

            // Initialize the number of tiles to be according the the number of columns and rows
            tiles = new Tile[numCols, numRows];

            // Goes through every line in the text past the first two
            for (int rows = 0; rows < numRows; rows++)
            {
                // Sets the row data to be split by commas to siginify a new column
                rowData = inFile.ReadLine().Split(',');

                // Goes through every column in the row
                for (int cols = 0; cols < rowData.Length; cols++)
                {
                    // If the data in the column is not blank
                    if (rowData[cols] != "")
                    {
                        // According to each character in the text check to see for the 0/1
                        // for each property and set the property to be true or false accordingly
                        if (rowData[cols][rowData[cols].Length - 6] == '1')
                        {
                            isPlantSpot = true;
                        }
                        else
                        {
                            isPlantSpot = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 5] == '1')
                        {
                            isSaveSpot = true;
                        }
                        else
                        {
                            isSaveSpot = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 4] == '1')
                        {
                            isSolid = true;
                        }
                        else
                        {
                            isSolid = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 3] == '1')
                        {
                            isCTSpawnPoint = true;
                        }
                        else
                        {
                            isCTSpawnPoint = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 2] == '1')
                        {
                            isTSpawnPoint = true;
                        }
                        else
                        {
                            isTSpawnPoint = false;
                        }
                        if (rowData[cols][rowData[cols].Length - 1] == '1')
                        {
                            isSiteDefencePoint = true;
                        }
                        else
                        {
                            isSiteDefencePoint = false;
                        }

                        // Initialize each property of the tile
                        tiles[cols, rows] = new Tile(Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 6)), isSaveSpot, isPlantSpot, isSolid, isCTSpawnPoint, isTSpawnPoint, isSiteDefencePoint);
                    }
                }
            }

            // Close the file
            inFile.Close();
        }
        public void SaveFile(string fileName)
        {
            //
            StreamWriter outFile = File.CreateText(fileName);

            // Write the number of columns in a line
            outFile.WriteLine(numCols);

            //Write the number of rows in a line
            outFile.WriteLine(numRows);

            // Goes through every row and each column for the row
            for (int rows = 0; rows < numRows; rows++)
            {
                for (int cols = 0; cols < numCols; cols++)
                {
                    // If the data is not blank for the tile being checked
                    if (tiles[cols, rows] != null)
                    {
                        // Write the tile type
                        outFile.Write(Convert.ToString(tiles[cols, rows].TileType));

                        // Write each property of the tile using "1" or "0" representing true or false
                        if (tiles[cols, rows].IsPlantSpot)
                        {
                            outFile.Write("1");
                        }
                        else
                        {
                            outFile.Write("0");
                        }
                        if (tiles[cols, rows].IsSaveSpot)
                        {
                            outFile.Write("1");
                        }
                        else
                        {
                            outFile.Write("0");
                        }
                        if (tiles[cols, rows].IsSolid)
                        {
                            outFile.Write("0");
                        }
                        else
                        {
                            outFile.Write("0");
                        }
                        if (tiles[cols, rows].IsCTSpawnPoint)
                        {
                            outFile.Write("1");
                        }
                        else
                        {
                            outFile.Write("0");
                        }
                        if (tiles[cols, rows].IsTSpawnPoint)
                        {
                            outFile.Write("1");
                        }
                        else
                        {
                            outFile.Write("0");
                        }
                        if (tiles[cols, rows].IsSiteDefencePoint)
                        {
                            outFile.Write("1");
                        }
                        else
                        {
                            outFile.Write("0");
                        }
                    }

                    // Write a comma before going to the next column
                    outFile.Write(",");
                }

                // Start a new line to write on
                outFile.WriteLine();
            }

            // Close the file
            outFile.Close();
        }
    }
}
