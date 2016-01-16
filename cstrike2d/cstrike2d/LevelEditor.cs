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

        int numRows = 20;
        int numCols = 20;

        private InputManager input;
        private AudioManager audio;
        private CStrike2D driver;

        Rectangle tileSetOffset = new Rectangle(50, 50, 256, 320);
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
                    //SelectedTileLoc = new Vector2((float)(Math.Floor(input.MousePosition.X - tileSetOffset.X)), (float)(Math.Floor(input.MousePosition.Y - tileSetOffset.Y)));
                    selectedTile = (int)((input.MousePosition.Y - tileSetOffset.Y) / tileSize) * 8 +
                                   (int)((input.MousePosition.X - tileSetOffset.X) / tileSize);
                }

                if (input.MousePosition.X > tileSetOffset.X &&
                    input.MousePosition.X < tileSetOffset.X + tileSetOffset.Width &&
                    input.MousePosition.Y > tileSetOffset.Y &&
                    input.MousePosition.Y < tileSetOffset.Y + tileSetOffset.Height)
                {
                    Vector2 placedTileLocation = new Vector2(((input.MousePosition.X - tileSetOffset.X) / tileSize), (input.MousePosition.Y - tileSetOffset.Y) / tileSize);
                    //tiles[(int)((input.MousePosition.X - tileSetOffset.X) / tileSize), 
                    //      (int)((input.MousePosition.Y - tileSetOffset.Y) / tileSize)] = selectedTile;
                }
            }
            
        }

        public void DrawWorld(SpriteBatch sb)
        {
            for (int rows = 0; rows < numRows; rows++)
            {
                for (int cols = 0; cols < numCols; cols++)
                {
                    
                }
            }
        }

        public void DrawUI(SpriteBatch sb)
        {
            sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);
            sb.DrawString(driver.Assets.DefaultFont, "" + selectedTile, new Vector2 (0,0), Color.White );
            
            //sb.DrawString(driver.Assets.DefaultFont,"X" , SelectedTileLoc, Color.White);
        }
    }
}
