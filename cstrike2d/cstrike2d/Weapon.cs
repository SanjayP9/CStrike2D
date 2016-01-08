using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    class Weapon : Entity
    {
        public Weapon(Assets assets) : base(assets)
        {

        }

        public override int DrawOrder { get; protected set; }
        public override Vector2 Position { get; protected set; }
        public override Rectangle Dimensions { get; protected set; }

        public override void Update(float gameTime)
        {

        }

        public override void Draw(SpriteBatch sb)
        {

        }
    }
}
