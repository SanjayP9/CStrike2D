// Author: Shawn Verma
// File Name: LevelEditor.cs
// Project Name: Global Offensive
// Creation Date: Dec 31st, 2015
// Modified Date: Jan 19th, 2016
// Description: Allows ability to edit and view maps using various features including: undo, place/remove, pan, zoom, place nodes,
//              save file, load file, set all tiles, delete all tiles etc.
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace CStrike2D
{
    public class LevelEditor
    {
        // Stores the tiles 
        Tile[,] tiles = new Tile[75, 50];

        // Stores the undo tiles
        Tile[,] undoTiles = new Tile[75, 50];

        // Stores the tile color
        Color tileColor = Color.White;

        // Stores the selected tile of the tile set
        byte selectedTile;

        // Stores the constants of the tile size and the width/height of the tile set
        public const byte TILE_SIZE = 32;
        const byte TILE_SET_WIDTH = 8;
        const byte TILES_SET_HEIGHT = 10;
        const float CAMERA_MOVE_SPEED = 7;

        // Stores all the properties of a tile in a string
        string property = "";

        // Variables used to store the number of columns and rows
        int numCols = 75;
        int numRows = 50;
        
        // Stores the position of the mouse according to the map
        Vector2 mouseMap;

        //  Stores the position of the tile on the map
        Vector2 mapTilePos;

        // Bool to show or not show the edit view
        bool showEditView = true;

        private InputManager input;
        private CStrike2D driver;

        // Stores the tile set of set position
        Rectangle tileSetOffset = new Rectangle(50, 50, 256, 320);

        // Stores the map area
        Rectangle mapArea = new Rectangle(0, 0, 2400, 1600);

        Rectangle destRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

        /// <summary>
        /// Represents states of the level editor
        /// </summary>
        public enum EditorStates
        {
            Edit,
            ShowHotkeys,
            Exit
        };

        // Stores the current enum state
        public EditorStates CurrentState { get; private set; }

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            CurrentState = EditorStates.Edit;
            driver.Model.Camera.Position = new Vector2(225, 250);
        }
        
        public void Update(float gameTime)
        {
            // Updates logic according to the state
            switch (CurrentState)
            {
                case EditorStates.Edit:

                    // Holds the mouse position according to the map
                    mouseMap = input.ScreenToWorld(input.MousePosition, driver.Model.Camera, driver.Model.Center);

                    // Reset the property word
                    property = "";

                    // If mouse is over the tile set
                    if (input.MousePosition.X > tileSetOffset.X &&
                        input.MousePosition.X < tileSetOffset.X + tileSetOffset.Width &&
                        input.MousePosition.Y > tileSetOffset.Y &&
                        input.MousePosition.Y < tileSetOffset.Y + tileSetOffset.Height &&
                        showEditView)
                    {
                        // If left mouse is clicked
                        if (input.LeftClick())
                        {
                            // Set the selected tile to be the one that they have clicked
                            selectedTile = (byte)((int)((input.MousePosition.Y - tileSetOffset.Y) / TILE_SIZE) * TILE_SET_WIDTH +
                                           (int)((input.MousePosition.X - tileSetOffset.X) / TILE_SIZE));
                        }
                    }
                    // If the mouse is over the map region
                    else if (mouseMap.X > mapArea.X &&
                             mouseMap.X < mapArea.X + mapArea.Width &&
                             mouseMap.Y > mapArea.Y &&
                             mouseMap.Y < mapArea.Y + mapArea.Height)
                    {
                        // Find the tile the mouse is on
                        mapTilePos = new Vector2(((mouseMap.X - mapArea.X) / TILE_SIZE), (mouseMap.Y - mapArea.Y) / TILE_SIZE);

                        // If the left mouse button is held
                        if (input.LeftHold())
                        {
                            // Set the selected tile to be in that map tile position according to the mouse keeping the properties if any
                            if (tiles[(int)mapTilePos.X, (int)mapTilePos.Y] != null &&
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].TileType != selectedTile)
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetTileType(selectedTile);
                            }
                            else if (tiles[(int)mapTilePos.X, (int)mapTilePos.Y] == null)
                            {
                                TempSave();

                                tiles[(int) mapTilePos.X, (int) mapTilePos.Y] = new Tile(selectedTile, Tile.NO_PROPERTY,
                                      (int) mapTilePos.X, (int) mapTilePos.Y, destRect);
                            }
                        }
                        // If the right mouse button is held
                        else if (input.RightHold() && tiles[(int)mapTilePos.X, (int)mapTilePos.Y] != null)
                        {
                            TempSave();
                            // Set the tile that the mouse is on to be empty
                            tiles[(int)mapTilePos.X, (int)mapTilePos.Y] = null;
                        }

                        // If the tile the mouse is on is not null set a property according to the key press
                        if (tiles[(int)mapTilePos.X, (int)mapTilePos.Y] != null)
                        {
                            if (input.Tapped(Keys.D0))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.NO_PROPERTY);
                            }
                            else if (input.Tapped(Keys.D1))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.SOLID);
                            }
                            else if (input.Tapped(Keys.D2))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.A_PLANT_SPOT);
                            }
                            else if (input.Tapped(Keys.D3))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.B_PLANT_SPOT);
                            }
                            else if (input.Tapped(Keys.D4))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.SAVE_SPOT);
                            }
                            else if (input.Tapped(Keys.D5))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.CT_SPAWN_POINT);
                            }
                            else if (input.Tapped(Keys.D6))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.T_SPAWN_POINT);
                            }
                            else if (input.Tapped(Keys.D7))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.A_SITE_DEFENCE_POINT);
                            }
                            else if (input.Tapped(Keys.D8))
                            {
                                TempSave();
                                tiles[(int)mapTilePos.X, (int)mapTilePos.Y].SetProperty(Tile.B_SITE_DEFENCE_POINT);
                            }

                            // Add the specified properties to the properties string if that property is true
                            switch (tiles[(int)mapTilePos.X, (int)mapTilePos.Y].Property)
                            {
                                case Tile.NO_PROPERTY:
                                    property = "No Property";
                                    break;
                                case Tile.SOLID:
                                    property = "Collidable Tile";
                                    break;
                                case Tile.A_PLANT_SPOT:
                                    property = "A Plant Tile";
                                    break;
                                case Tile.B_PLANT_SPOT:
                                    property = "B Plant Tile";
                                    break;
                                case Tile.SAVE_SPOT:
                                    property = "Save Tile";
                                    break;
                                case Tile.CT_SPAWN_POINT:
                                    property = "CT Spawn Tile";
                                    break;
                                case Tile.T_SPAWN_POINT:
                                    property = "T Spawn Tile";
                                    break;
                                case Tile.A_SITE_DEFENCE_POINT:
                                    property = "A Site Defence Tile";
                                    break;
                                case Tile.B_SITE_DEFENCE_POINT:
                                    property = "B Site Defence Tile";
                                    break;
                            }
                        }
                    }

                    // If the left control key is held
                    if (input.Held(Keys.LeftControl))
                    {
                        // Prompts the user to save using a dialog prompt for the location
                        // When the S key is pressed
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
                        // Prompts the user to open a file using a dialog promt for the location
                        // When the O key is pressed
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
                        // If 0 key is pressed reset the camera zoom
                        else if (input.Tapped(Keys.D0))
                        {
                            driver.Model.Camera.ResetZoom();
                        }
                        // If the A key is pressed set all the tiles to be the selected tile
                        else if (input.Tapped(Keys.A))
                        {
                            TempSave();
                            for (int col = 0; col < numCols; col++)
                            {
                                for (int row = 0; row < numRows; row++)
                                {
                                    tiles[col, row] = new Tile(selectedTile, Tile.NO_PROPERTY, col, row, mapArea);
                                }
                            }
                        }
                        // If the delete key is pressed it removes all tiles placed
                        else if (input.Tapped(Keys.Delete))
                        {
                            TempSave();
                            tiles = new Tile[numCols, numRows];
                        }
                        // If the undo key is pressed it undoes the last action
                        else if (input.Tapped(Keys.Z))
                        {
                            Undo();
                        }
                    }
                    // Move the camera left when the A key is held
                    else if (input.Held(Keys.A))
                    {
                        driver.Model.Camera.Position.X -= CAMERA_MOVE_SPEED;
                    }
                    // Move the camera right when the D key is held
                    else if (input.Held(Keys.D))
                    {
                        driver.Model.Camera.Position.X += CAMERA_MOVE_SPEED;
                    }
                    // Move the camera up when the W key is held
                    if (input.Held(Keys.W))
                    {
                        driver.Model.Camera.Position.Y -= CAMERA_MOVE_SPEED;
                    }
                    // Move the camera down when the S key is held
                    else if (input.Held(Keys.S))
                    {
                        driver.Model.Camera.Position.Y += CAMERA_MOVE_SPEED;
                    }

                    // If the scroll up key is used zoom in
                    if (input.ScrollUp())
                    {
                        driver.Model.Camera.IncreaseZoom();
                    }
                    // If the scroll down key is used zoom out
                    else if (input.ScrollDown())
                    {
                        driver.Model.Camera.DecreaseZoom();
                    }

                    // If the tab key is pressed toggle the show edit view
                    if (input.Tapped(Keys.Tab))
                    {
                        showEditView = !showEditView;
                    }

                    // If the F2 key is pressed change the state to the hot key state
                    if (input.Tapped(Keys.F12))
                    {
                        CurrentState = EditorStates.ShowHotkeys;
                    }

                    if (input.Tapped(Keys.Escape))
                    {
                        CurrentState = EditorStates.Exit;
                    }
                    break;

                case EditorStates.ShowHotkeys:
                    // If the F2 key is pressed change the state to the edit state
                    if (input.Tapped(Keys.F12))
                    {
                        CurrentState = EditorStates.Edit;
                    }
                    break;
            }
        }

        /// <summary>
        /// Temporary saves the map
        /// </summary>
        public void TempSave()
        {
            for (int row = 0; row < undoTiles.GetLength(1); row++)
            {
                for (int col = 0; col < undoTiles.GetLength(0); col++)
                {
                    undoTiles[col, row] = tiles[col, row];
                }
            }
        }

        /// <summary>
        /// Undoes the last action
        /// </summary>
        public void Undo()
        {
            for (int row = 0; row < undoTiles.GetLength(1); row++)
            {
                for (int col = 0; col < undoTiles.GetLength(0); col++)
                {
                    tiles[col, row] = undoTiles[col, row];
                }
            }
        }

        /// <summary>
        /// Draws the map according to the camea position
        /// </summary>
        /// <param name="sb">Passes through spritebatch instance in order to use draw method</param>
        public void DrawWorld(SpriteBatch sb)
        {
            switch (CurrentState)
            {
                case EditorStates.Edit:
                    // Draws the background of the map
                    sb.Draw(driver.Assets.PixelTexture, mapArea, Color.CornflowerBlue);
                    Vector2 screenDimensions = input.ScreenToWorld(driver.Model.Dimensions, driver.Model.Camera, driver.Model.Center);
                    Vector2 screenStart = input.ScreenToWorld(new Vector2(0, 0), driver.Model.Camera, driver.Model.Center);


                    Rectangle srcRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

                   /* 
                    // Draws all tiles placed on the map and colors the tile according to the property
                    for (int col = 0; col < numCols; col++)
                    {
                        for (int row = 0; row < numRows; row++)
                        {
                            if (tiles[col, row] != null &&
                                col * TILE_SIZE < screenDimensions.X - mapArea.X &&
                                col * (TILE_SIZE * 2) > screenStart.X - mapArea.X &&
                                row * TILE_SIZE < screenDimensions.Y - mapArea.Y &&
                                row * (TILE_SIZE * 2) > screenStart.Y - mapArea.Y)
                            {
                                // If show edit view is true change the color of the tile according to the property
                                if (showEditView)
                                {
                                    switch (tiles[col, row].Property)
                                    {
                                        case Tile.NO_PROPERTY:
                                            tileColor = Color.White;
                                            break;
                                        case Tile.SOLID:
                                            tileColor = Color.Red;
                                            break;
                                        case Tile.A_PLANT_SPOT:
                                            tileColor = Color.Yellow;
                                            break;
                                        case Tile.B_PLANT_SPOT:
                                            tileColor = Color.Yellow;
                                            break;
                                        case Tile.SAVE_SPOT:
                                            tileColor = Color.Green;
                                            break;
                                        case Tile.CT_SPAWN_POINT:
                                            tileColor = Color.Blue;
                                            break;
                                        case Tile.T_SPAWN_POINT:
                                            tileColor = Color.Orange;
                                            break;
                                        case Tile.A_SITE_DEFENCE_POINT:
                                            tileColor = Color.Purple;
                                            break;
                                        case Tile.B_SITE_DEFENCE_POINT:
                                            tileColor = Color.Purple;
                                            break;
                                    }
                                }
                                else
                                {
                                    tileColor = Color.White;
                                }

                                // Find the source rectangle and the destination retangle
                                srcRect.X = (tiles[col, row].TileType % 8 * TILE_SIZE);
                                srcRect.Y = (tiles[col, row].TileType / 8 * TILE_SIZE);
                                destRect.X = col * TILE_SIZE + mapArea.X;
                                destRect.Y = row * TILE_SIZE + mapArea.Y;

                                // Draw the tile
                                sb.Draw(driver.Assets.TileSet, destRect, srcRect, tileColor);
                            }
                        }
                    }
                    */

                    
                    foreach (Tile tile in tiles)
                    {
                        if (tile != null)
                        {
                            tile.Draw(sb, driver.Assets.TileSet, showEditView);
                        }
                    }
                   

                    // If show edit view is true
                    if (showEditView)
                    {
                        // Draws a grid line for the map each line being 1 pixel thick
                        for (int col = 0; col <= numCols; col++)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(mapArea.X + col * TILE_SIZE), (int)mapArea.Y, 1, (int)mapArea.Height), Color.Black);
                        }
                        for (int row = 0; row <= numRows; row++)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)mapArea.X, (int)(mapArea.Y + row * TILE_SIZE), (int)mapArea.Width, 1), Color.Black);
                        }
                    }
                    break;
                case EditorStates.ShowHotkeys:
                    
                    break;
            }
        }

        /// <summary>
        /// Draws the UI according to the screen dimensions
        /// </summary>
        /// <param name="sb">Passes through spritebatch instance in order to use draw method</param>
        public void DrawUI(SpriteBatch sb)
        {
            switch (CurrentState)
            {
                case EditorStates.Edit:

                    // Draw the UI which includes map tile index, the selected tile #, and the properties of the tile the mouse is on
                    sb.DrawString(driver.Assets.DefaultFont, "Map Tile Index: " + Math.Floor(mapTilePos.X) + "|" + Math.Floor(mapTilePos.Y), new Vector2(200, 0), Color.White);
                    sb.DrawString(driver.Assets.DefaultFont, "Selected Tile #: " + selectedTile, new Vector2(25, 0), Color.White);
                    sb.DrawString(driver.Assets.DefaultFont, "Property: " + property, new Vector2(385, 0), Color.White);
                    sb.DrawString(driver.Assets.DefaultFont, "Press 'F12' to view controls", new Vector2(25, 675), Color.White);

                    // If show edit view is true draw the tile set and grid lines for it
                    if (showEditView)
                    {
                        // Draw the tile set
                        sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);

                        // Drawn the grid line for the tile set each line being 1 pixel thick
                        for (int col = 0; col <= TILE_SET_WIDTH; col++)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)(tileSetOffset.X + col * TILE_SIZE), (int)tileSetOffset.Y, 1, (int)tileSetOffset.Height), Color.Black);
                        }
                        for (int row = 0; row <= TILES_SET_HEIGHT; row++)
                        {
                            sb.Draw(driver.Assets.PixelTexture, new Rectangle((int)tileSetOffset.X, (int)(tileSetOffset.Y + row * TILE_SIZE), (int)tileSetOffset.Width, 1), Color.Black);
                        }
                    }
                    break;

                    // 
                case EditorStates.ShowHotkeys:
                    sb.DrawString(driver.Assets.DefaultFont, "MOUSE1 -> SELECT/PLACE TILE\n\nMOUSE2-> REMOVE TILE\n\nCTRL + S -> SAVE FILE\n\nCTRL + O -> OPEN FILE\n\nCTRL + A -> SET ALL TILES\n\nCTRL + DEL -> DELETE ALL TILES\n\nCTRL + Z -> Undo\n\nSCROLL WHEEL -> ZOOM\n\nWASD -> MOVE CAMERA\n\nTAB -> TOGGLE EDIT VIEW", new Vector2(250, 75), Color.White);
                    sb.DrawString(driver.Assets.DefaultFont, "1 -> COLLIDABLE\n\n2-> A SITE PLANT SPOT\n\n3 -> B SITE PLANT SPOT\n\n4 -> SAVE SPOT\n\n5 - > CT SPAWN POINT\n\n6 -> T SPAWN POINT\n\n7 -> A SITE DEFENCE POINT\n\n8 -> B SITE DEFENCE POINT\n\n\n\n0 -> RESET PROPERTY", new Vector2(825, 75), Color.White);
                    sb.DrawString(driver.Assets.DefaultFont, "Press 'F12' to return to editor", new Vector2(515, 675), Color.White);
                    //sb.Draw(driver.Assets.PixelTexture, new Rectangle(700, 75, 50, 50), Color.White);
                    break;
            }
        }
        public void LoadFile(string filePath)
        {
            // Creates a stream reader instance of the text file
            StreamReader inFile = File.OpenText(filePath);

            // Stores the data for a single line as a time
            string[] rowData;

            // Checks the first and second line of the text to set the number of columns and the number of rows
            numCols = Convert.ToInt32(inFile.ReadLine());
            numRows = Convert.ToInt32(inFile.ReadLine());

            // Changes the map area according to the number of columns and rows
            mapArea.Width = TILE_SIZE * numCols;
            mapArea.Height = TILE_SIZE * numRows;

            // Sets the max indexes of the number of columns and rows
            undoTiles = new Tile[numCols, numRows];
            TempSave();
            
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
                        // Initialize each property of the tile
                        tiles[cols, rows] =
                            new Tile((byte) Convert.ToInt32(rowData[cols].Substring(0, rowData[cols].Length - 1)),
                                (byte) Convert.ToInt32(rowData[cols].Substring(rowData[cols].Length - 1, 1)), 
                                cols, rows, mapArea);
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
                        // Write the tile type followed by the property
                        outFile.Write(Convert.ToString(tiles[cols, rows].TileType) + Convert.ToString(tiles[cols, rows].Property));
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
        private void AddColumn()
        {
            TempSave();
            numCols++;
            tiles = new Tile[numCols, numRows];
            mapArea.Width = TILE_SIZE * numCols;
            //mapArea.Height = TILE_SIZE * numRows;
            Undo();
        }
    }
}
