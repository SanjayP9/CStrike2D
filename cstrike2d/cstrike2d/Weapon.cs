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
        public Texture2D WeaponTexture { get; private set; }

        public Weapon(byte weaponID, Texture2D weaponTexture, Assets assets) : base(assets)
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
