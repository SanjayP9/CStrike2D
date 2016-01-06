using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    class LevelEditor
    {
        Tile[,] tiles;
        Tile selectedTile;

        int columns = 20;
        int rows = 20;
        public LevelEditor()
        {

        }

        public void Update(InputManager input)
        {
            tiles = new Tile[columns, rows];
            if(input.MousePosition == (new Vector2(0,0)))
            {
                
            }
            tiles[0, 0] = selectedTile;
        }
    }
}
