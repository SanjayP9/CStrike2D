using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CStrike2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cstrike2d
{
    public class Player : Entity
    {
        public override int DrawOrder { get; protected set; }
        public override Vector2 Position { get; protected set; }
        public override Rectangle Dimensions { get; protected set; }

        public Player()
        {
            
        }

        public override void Update(float gameTime)
        { 

        }

        public override void Draw(SpriteBatch sb, Assets assets)
        {
        }
    }
}
