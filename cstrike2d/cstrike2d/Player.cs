using System;
using System.Diagnostics;
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

        public Weapon PrimaryWeapon { get; private set; }

        public Weapon SecondaryWeapon { get; private set; }

        public Weapon Knife { get; private set; }

        public Weapon CurrentWeapon { get; private set; }

        public NetInterface.Team Team { get; private set; }

        private Assets assets;

        public Player(string name, Vector2 position, short playerID, byte team, Assets assets) : base(assets)
        {
            Position = position;
            Name = name;
            PlayerID = playerID;
            Team = NetInterface.GetTeam(team);
            this.assets = assets;
            Knife = new Weapon(-1, NetInterface.WEAPON_KNIFE, position, assets);
            CurrentWeapon = Knife;
        }

        /// <summary>
        /// Switches the current weapon to the target one
        /// </summary>
        /// <param name="weapon"></param>
        public void SwitchWeapon(WeaponInfo.WeaponType weapon)
        {
            switch (weapon)
            {
                case WeaponInfo.WeaponType.Primary:
                    CurrentWeapon = PrimaryWeapon;
                    break;
                case WeaponInfo.WeaponType.Secondary:
                    CurrentWeapon = SecondaryWeapon;
                    break;
            }
        }

        /// <summary>
        /// Used by the network to set another player's
        /// current weapon
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="weaponID"></param>
        public void SetWeapon(short entityID, short weaponID)
        {
            CurrentWeapon = new Weapon(entityID, weaponID, position, assets);
        }

        public void SetPrimaryWeapon(short entityID, short weapon)
        {
            PrimaryWeapon = new Weapon(entityID, weapon, position, assets);
        }

        public void SetSecondaryWeapon(short entityID, short weapon)
        {
            SecondaryWeapon = new Weapon(entityID, weapon, position, assets);
        }

        public void SetTeam(NetInterface.Team team)
        {
            Team = team;
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void SetRot(float rotation)
        {
            this.rotation = rotation;
            Debug.WriteLine(rotation);
        }

        public void Fire()
        {
            
        }

        public void Move(byte direction)
        {
            switch (direction)
            {
                case NetInterface.MOVE_UP: // UP
                    position.Y -= 5f;
                    break;
                case NetInterface.MOVE_DOWN: // DOWN
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_LEFT: // LEFT
                    position.X -= 5f;
                    break;
                case NetInterface.MOVE_RIGHT: // RIGHT
                    position.X += 5f;
                    break;
                case NetInterface.MOVE_UPRIGHT:
                    position.X += 5f;
                    position.Y -= 5f;
                    break;
                case NetInterface.MOVE_DOWNRIGHT:
                    position.X += 5f;
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_DOWNLEFT:
                    position.X -= 5f;
                    position.Y += 5f;
                    break;
                case NetInterface.MOVE_UPLEFT:
                    position.X -= 5f;
                    position.Y -= 5f;
                    break;
            }
        }

        public override void Update(float gameTime)
        {
            CurrentWeapon.Update(gameTime);
            CurrentWeapon.SetDirection(rotation);
            CurrentWeapon.SetPosition(position);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Team != NetInterface.Team.Spectator)
            {
                sb.Draw(Assets.CTTexture, position, new Rectangle(0, 0, 32, 32), Color.White, 1.57f + rotation,
                    new Vector2(16, 16),
                    1f, SpriteEffects.None, 0);

                sb.DrawString(Assets.DefaultFont, Name, new Vector2(position.X, position.Y - 50),
                    Team == NetInterface.Team.CT ? Color.Blue : Color.Red);

                CurrentWeapon.Draw(sb);
            }
        }
    }
}