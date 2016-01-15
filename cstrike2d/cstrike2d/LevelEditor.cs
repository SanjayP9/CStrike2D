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
        Tile selectedTile;

        int columns = 20;
        int rows = 20;

        private InputManager input;
        private AudioManager audio;
        private CStrike2D driver;

        Vector2 tileSetOffset = Vector2.Zero;

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            audio = model.AudioManager;
        }

        public void Update(float gameTime)
        {
            //if(input.MousePosition.X)
        }

        public void DrawWorld(SpriteBatch sb)
        {
            sb.Draw(driver.Assets.TileSet, tileSetOffset, Color.White);
        }

        public void DrawUI(SpriteBatch sb)
        {
            
        }
    }
}
