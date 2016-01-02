using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
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

        public override void Draw(SpriteBatch sb)
        {
        }
    }
}
