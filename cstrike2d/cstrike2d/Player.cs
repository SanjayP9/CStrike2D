using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public sealed class Player : Entity
    {
        public override int DrawOrder { get; protected set; }
        public override Vector2 Position { get; protected set; }
        public override Rectangle Dimensions { get; protected set; }

        private int playerType;

        public string Name { get; private set; }

        public Player(string name, Vector2 position)
        {
            Position = position;
            Name = name;
        }

        public void SetPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        public override void Update(float gameTime)
        {
             
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Assets.CTTexture, Position, Color.White);
        }
    }
}
