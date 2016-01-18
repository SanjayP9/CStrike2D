using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;
namespace CStrike2D
{
    public class LevelEditor
    {
        // Stores the tiles 
        Tile[,] tiles = new Tile[75, 50];

        //
        Tile[,] prevTiles = new Tile[75, 50];


        // Stores the selected tile of the tile set
        int selectedTile;

        // Stores the constants of the tile size and the width/height of the tile set
        const int TILE_SIZE = 32;
        const int TILE_SET_WIDTH = 8;
        const int TILES_SET_HEIGHT = 10;

        // Stores all the properties of a tile in a string
        string properties = "";

        // Variables used to store the number of columns and rows
        int numCols = 75;
        int numRows = 50;
        
        // Stores the position of the mouse according to the map
        Vector2 mouseMap;

        // 
        Vector2 placedTilePos;

        bool displayTileSet = true;

        // Stores the status of the properties of a tile
        bool isPlantSpot;
        bool isSaveSpot;
        bool isSolid;
        bool isCTSpawnPoint;
        bool isTSpawnPoint;
        bool isSiteDefencePoint;

        private InputManager input;
        private AudioManager audio;
        private CStrike2D driver;

        // Stores the tile set of set position
        Rectangle tileSetOffset = new Rectangle(50, 100, 256, 320);

        // Stores the map area
        Rectangle mapArea = new Rectangle(-225, -250, 2400, 1600);

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            audio = model.AudioManager;
        }

        public void Update(float gameTime)
        {
            // Holds the mouse position according to the world
            mouseMap = input.ScreenToWorld(input.MousePosition, driver.Model.Camera, driver.Model.Center);

            // Reset the properties
            properties = "";
            // If mouse is over the tile set
            if (input.MousePosition.X > tileSetOffset.X &&
                input.MousePosition.X < tileSetOffset.X + tileSetOffset.Width &&
                input.MousePosition.Y > tileSetOffset.Y &&
                input.MousePosition.Y < tileSetOffset.Y + tileSetOffset.Height &&
                displayTileSet)
            {
                // If left mouse is clicked
                if (input.LeftClick())
                {
                    // Set the selected tile to be the one that they have clicked
                    selectedTile = (int)((input.MousePosition.Y - tileSetOffset.Y) / TILE_SIZE) * TILE_SET_WIDTH +
                                   (int)((input.MousePosition.X - tileSetOffset.X) / TILE_SIZE);
                }
            }
            // If the mouse is over the map region
            else if (mouseMap.X > mapArea.X &&
                mouseMap.X < mapArea.X + mapArea.Width &&
                mouseMap.Y > mapArea.Y &&
                mouseMap.Y < mapArea.Y + mapArea.Height)
            {
                // Find the tile the mouse is on
                placedTilePos = new Vector2(((mouseMap.X - mapArea.X) / TILE_SIZE), (mouseMap.Y - mapArea.Y) / TILE_SIZE);

                // If the left mouse button is held
                if (input.LeftHold())
                {
                    // Set the selected tile to be in that tile position according to the mouse keeping the properties if any
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null &&
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].TileType != selectedTile)
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetTileType(selectedTile);
                    }
                    else if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] == null)
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = new Tile(selectedTile, false, false, false, false, false, false);
                    }
                }
                // If the right mouse button is held
                else if (input.RightClick())
                {
                    BackUp();
                    // Set the tile that the mouse is on to be empty
                    tiles[(int)placedTilePos.X, (int)placedTilePos.Y] = null;
                }

                // If the tile the mouse is on is not null
                if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y] != null)
                {
                    // 
                    if (input.Tapped(Keys.D1))
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsPlantSpot(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsPlantSpot);
                    }
                    // 
                    else if (input.Tapped(Keys.D2))
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsSaveSpot(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSaveSpot);
                    }
                    // 
                    else if (input.Tapped(Keys.D3))
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsSolid(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSolid);
                    }
                    // 
                    else if (input.Tapped(Keys.D4))
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsCTSpawnPoint(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsCTSpawnPoint);
                    }
                    // 
                    else if (input.Tapped(Keys.D5))
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsTSpawnPoint(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsTSpawnPoint);
                    }
                    // 
                    else if (input.Tapped(Keys.D6))
                    {
                        BackUp();
                        tiles[(int)placedTilePos.X, (int)placedTilePos.Y].SetIsSiteDefencePoint(!tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSiteDefencePoint);
                    }

                    // Add the specified properties to the properties string if that property is true
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsPlantSpot)
                    {
                        properties += "Plant Spot, ";
                    }
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSaveSpot)
                    {
                        properties += "Save Spot, ";
                    }
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSolid)
                    {
                        properties += "Collidable, ";
                    }
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsCTSpawnPoint)
                    {
                        properties += "CT Spawn Point, ";
                    }
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsTSpawnPoint)
                    {
                        properties += "T Spawn Point, ";
                    }
                    if (tiles[(int)placedTilePos.X, (int)placedTilePos.Y].IsSiteDefencePoint)
                    {
                        properties += "Defence Point ";
                    }
                }
            }
            if (input.Held(Keys.LeftControl))
            {
                if (input.Tapped(Keys.S))
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Map Files (.txt)|*.txt";
                    saveDialog.FilterIndex = 1;

                    DialogResult result = saveDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        SaveFile(saveDialog.FileName);
                    }
                }
                else if (input.Tapped(Keys.O))
                {
                    OpenFileDialog openDialog = new OpenFileDialog();
                    openDialog.Filter = "Map Files (.txt)|*.txt";
                    openDialog.FilterIndex = 1;
                    openDialog.Multiselect = false;

                    DialogResult result = openDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        LoadFile(openDialog.FileName);
                    }
                }
                else if (input.Tapped(Keys.D0))
                {
                    driver.Model.Camera.ResetZoom();
                }
                else if (input.Tapped(Keys.A))
                {
                    BackUp();
                    for (int x = 0; x < numCols; x++)
                    {
                        for (int y = 0; y < numRows; y++)
                        {
                            tiles[x, y] = new Tile(selectedTile, false, false, false, false, false, false);
                        }
                    }
                }
                else if (input.Tapped(Keys.Delete))
                {
                    BackUp();
                    tiles = new Tile[numCols, numRows];
                }
                else if (input.Tapped(Keys.Z))
                {
                    Revert();
                }
            }
            // Move the camera left when the A key is held
            else if (input.Held(Keys.A))
            {
                driver.Model.Camera.Position.X -= 5;
            }
            // Move the camera right when the D key is held
            else if (input.Held(Keys.D))
            {
                driver.Model.Camera.Position.X += 5;
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
            if (input.ScrollUp())
            {
                driver.Model.Camera.IncreaseZoom();
            }
            else if (input.ScrollDown())
            {
                driver.Model.Camera.DecreaseZoom();
            }
            if (input.Tapped(Keys.Tab))
            {
                displayTileSet = !displayTileSet;
            }
        }

        public void BackUp()
        {
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    prevTiles[col, row] = tiles[col, row];
                }
            }
        }
        public void Revert()
        {
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    tiles[col, row] = prevTiles[col, row];
                }
            }
        }

        public void DrawWorld(SpriteBatch sb)
        {
            // Draws the background of the map
            sb.Draw(driver.Assets.PixelTexture, mapArea, Color.CornflowerBlue);
            Vector2 screenDimensions = input.ScreenToWorld(driver.Model.Dimensions, driver.Model.Camera, driver.Model.Center);
            Vector2 screenStart = input.ScreenToWorld(new Vector2(0, 0), driver.Model.Camera, driver.Model.Center);

            Rectangle destRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
            Rectangle srcRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

            // Draws all tiles placed and the properties highlighted overthem
            for (int x = 0; x < numCols; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    if (tiles[x, y] != null &&
                        (x * 32 < screenDimensions.X - mapArea.X &&
                         x * 32 + 32 > screenStart.X - mapArea.X) &&
                        (y * 32 < screenDimensions.Y - mapArea.Y &&
                         y * 32 + 32 > screenStart.Y - mapArea.Y)
                        )
                    {
                        srcRect.X = (tiles[x, y].TileType % 8 * TILE_SIZE);
                        srcRect.Y = (tiles[x, y].TileType / 8 * TILE_SIZE);
                        destRect.X = x*TILE_SIZE + mapArea.X;
                        destRect.Y = y*TILE_SIZE + mapArea.Y;
                        //Rectangle tileSrcRec = new Rectangle(srcX, srcY, (int)TILE_SIZE, (int)TILE_SIZE);
                        sb.Draw(driver.Assets.TileSet, destRect, srcRect, Color.White);
                        if (displayTileSet)
                        {
                            if (tiles[x, y].IsPlantSpot)
                            {
                                sb.Draw(driver.Assets.PixelTexture, destRect, Color.Yellow*0.5f);
                            }
                            if (tiles[x, y].IsSaveSpot)
                            {
                                sb.Draw(driver.Assets.PixelTexture, destRect, Color.Green*0.5f);
                            }
                            if (tiles[x, y].IsSolid)
                            {
                                sb.Draw(driver.Assets.PixelTexture, destRect, Color.Red*0.5f);
                            }
                            if (tiles[x, y].IsCTSpawnPoint)
                            {
                                sb.Draw(driver.Assets.PixelTexture, destRect, Color.Blue*0.5f);
                            }
                            if (tiles[x, y].IsTSpawnPoint)
                            {
                                sb.Draw(driver.Assets.PixelTexture, destRect, Color.Orange*0.5f);
                            }
                            if (tiles[x, y].IsSiteDefencePoint)
                            {
                                sb.Draw(driver.Assets.PixelTexture, destRect, Color.Purple*0.5f);
                            }
                        }
                    }
                }
            }
            if (displayTileSet)
            {
                // Draws a grid line for the map each line being 1 pixel thick
                for (int x = 0; x <= numCols; x++)
                {
                    sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(mapArea.X + x * TILE_SIZE), (int)mapArea.Y, 1, (int)mapArea.Height), Color.Black);
                }
                for (int y = 0; y <= numRows; y++)
                {
                    sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)mapArea.X, (int)(mapArea.Y + y * TILE_SIZE), (int)mapArea.Width, 1), Color.Black);
                }
            }
        }

        public void DrawUI(SpriteBatch sb)
        {
            // Drawn the UI, includes map tile index, the selected tile #, and the properties of the tile the mouse is on
            sb.DrawString(driver.Assets.DefaultFont, "Map Tile Index: " + Math.Floor(placedTilePos.X) + "|" + Math.Floor(placedTilePos.Y), new Vector2(175, 0), Color.White);
            sb.DrawString(driver.Assets.DefaultFont, "Selected Tile #: " + selectedTile, new Vector2(0, 0), Color.White);
            sb.DrawString(driver.Assets.DefaultFont, "Properties: " + properties, new Vector2(360, 0), Color.White);

            if (displayTileSet)
            {
                // Draw the tile set
                sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);

                // Drawn the grid line for the tile set each line being 1 pixel thick
                for (int x = 0; x <= TILE_SET_WIDTH; x++)
                {
                    sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(tileSetOffset.X + x * TILE_SIZE), (int)tileSetOffset.Y, 1, (int)tileSetOffset.Height), Color.Black);
                }
                for (int y = 0; y <= TILES_SET_HEIGHT; y++)
                {
                    sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)tileSetOffset.X, (int)(tileSetOffset.Y + y * TILE_SIZE), (int)tileSetOffset.Width, 1), Color.Black);
                }
            }
        }
        public void LoadFile(string filePath)
        {
            // Creates a stream reader instance of the text file
            StreamReader inFile = File.OpenText(filePath);

            // Count the amount of lines in the text file
            int lineCount = File.ReadLines(filePath).Count();

            // Stores the data for a single line as a time
            string[] rowData;

            // Checks the first and second line of the text to set the number of columns and the number of rows
            numCols = Convert.ToInt32(inFile.ReadLine());
            numRows = Convert.ToInt32(inFile.ReadLine());



            // Changes the placement area according to the number of columns and rows
            mapArea = new Rectangle(-200, -250, 32 * numCols, 32 * numRows);

            prevTiles = new Tile[numCols, numRows];
            BackUp();
            
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
                        tiles[cols, rows] = new Tile(Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 6)), isPlantSpot, isSaveSpot, isSolid, isCTSpawnPoint, isTSpawnPoint, isSiteDefencePoint);
                    }
                }
            }

            // Close the file
            inFile.Close();
        }
        private void SaveFile(string fileName)
        {
            // Creates a stream writer instance of the text file
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
                            outFile.Write("1");
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
