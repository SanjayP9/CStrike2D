using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public sealed class Player : Entity
    {
        public override int DrawOrder { get; protected set; }

        public override Vector2 Position
        {
            get { return position; }

            protected set { position = value; }
        }

        public override Rectangle Dimensions { get; protected set; }

        private Vector2 position;
        
        public int PlayerID { get; private set; }

        private int playerType;

        public string Name { get; private set; }

        public Player(string name, Vector2 position, int playerID)
        {
            Position = position;
            Name = name;
            PlayerID = playerID;
        }

        public void SetPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        public void Move(int direction)
        {
            switch (direction)
            {
                case 0: // UP
                    position.Y -= 0.5f;
                    break;
                case 1: // DOWN
                    position.Y += 0.5f;
                    break;
                case 2: // LEFT
                    position.X -= 0.5f;
                    break;
                case 3: // RIGHT
                    position.X += 0.5f;
                    break;
            }
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
