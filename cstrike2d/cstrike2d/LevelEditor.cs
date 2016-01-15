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

        public LevelEditor(CStrikeModel model)
        {
            driver = model.DriverInstance;
            input = model.Input;
            audio = model.AudioManager;
        }

        public void Update(float gameTime)
        {
            tiles = new Tile[columns, rows];
            if(input.MousePosition == (new Vector2(0,0)))
            {
                
            }
            tiles[0, 0] = selectedTile;
        }

        public void Draw(SpriteBatch sb)
        {
            
        }
    }
}
