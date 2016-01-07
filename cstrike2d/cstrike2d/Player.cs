using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public sealed class Player : Entity
    {
        // CLIENT-SIDE PLAYER ENTITY

        public override int DrawOrder { get; protected set; }

        public override Vector2 Position
        {
            get { return position; }

            protected set { position = value; }
        }

        public override Rectangle Dimensions { get; protected set; }

        private Vector2 position;
        private float rotation;

        public short PlayerID { get; private set; }

        private int playerType;

        public string Name { get; private set; }

        public Player(string name, Vector2 position, short playerID, Assets assets) : base(assets)
        {
            Position = position;
            Name = name;
            PlayerID = playerID;
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void SetRot(float rotation)
        {
            this.rotation = rotation;
        }

        public void Move(byte direction)
        {
            switch (direction)
            {
                case NetInterface.MOVE_UP: // UP
                    //position = Vector2.Lerp(position, new Vector2(position.X, position.Y - 5), 0.4f);
                    position.Y -= 5f;
                    break;
                case NetInterface.MOVE_DOWN: // DOWN
                    //position = Vector2.Lerp(position, new Vector2(position.X, position.Y + 5), 0.4f);
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_LEFT: // LEFT
                    //position = Vector2.Lerp(position, new Vector2(position.X - 5, position.Y), 0.4f);
                    position.X -= 5f;
                    break;
                case NetInterface.MOVE_RIGHT: // RIGHT
                    //position = Vector2.Lerp(position, new Vector2(position.X + 5, position.Y), 0.4f);
                    position.X += 5f;
                    break;
            }
        }

        public override void Update(float gameTime)
        {
             
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Assets.CTTexture, position, new Rectangle(0, 0, 32, 32), Color.White, 1.57f + rotation,
                new Vector2(16, 16),
                1f, SpriteEffects.None, 0);

            sb.DrawString(Assets.DefaultFont, Name, new Vector2(position.X, position.Y - 50), Color.White);
        }
    }
}
