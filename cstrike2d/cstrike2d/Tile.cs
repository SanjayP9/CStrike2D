using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace cstrike2d
{
    public class Tile
    {
        public bool Collidable { get; private set; }
        public bool ShootThrough { get; private set; }
        public int TileType { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Tile(int tileType, bool collidable, bool shootThrough)
        {
            TileType = tileType;
            Collidable = collidable;
            ShootThrough = shootThrough;
        }
    }
}
