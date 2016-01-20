// Author: Mark Voong
// File Name: ClientPlayer.cs
// Project Name: Global Offensive
// Creation Date: Jan 15th, 2016
// Modified Date: Jan 20th, 2016
// Description: Handles logic for the player.
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class ClientPlayer : Entity
    {
        public string UserName { get; private set; }
        public short Identifier { get; private set; }
        public short EntityIdentifier { get; private set; }

        public ServerClientInterface.PlayerState State { get; private set; }
        public ServerClientInterface.Team CurrentTeam { get; private set; }
        public int Health { get; private set; }
        public int Armor { get; private set; }

        // Weapons
        public ClientWeapon CurrentWeapon { get; private set; }
        public ClientWeapon PrimaryWeapon { get; private set; }
        public ClientWeapon SecondaryWeapon { get; private set; }
        public ClientWeapon Knife { get; private set; }

        private RayEmitter rayEmitter;

        // State

        public override int DrawOrder { get; protected set; }

        public override Rectangle Dimensions { get; protected set; }

        // Gets the position of the player
        public override Vector2 Position
        {
            get
            {
                return position;
            }
            protected set
            {
                position = value;
            }
        }
        /// <summary>
        /// Gets the rotation of the player
        /// </summary>
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        private Vector2 position;
        private float rotation;

        public ClientPlayer(string username, short identifier, Assets assets)
            : base(assets)
        {
            UserName = username;
            Identifier = identifier;
            CurrentTeam = ServerClientInterface.Team.Spectator;
            State = ServerClientInterface.PlayerState.Dead;
            Health = 100;
            Armor = 0;
            PrimaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this, assets);
            SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this, assets);
            Knife = new ClientWeapon(WeaponData.Weapon.Knife, this, assets);
            CurrentWeapon = Knife;
        }

        public override void Update(float gameTime)
        {
            CurrentWeapon.Update(gameTime);
            //PrimaryWeapon.Update(gameTime);
            //SecondaryWeapon.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (CurrentTeam != ServerClientInterface.Team.Spectator && State != ServerClientInterface.PlayerState.Dead)
            {
                sb.Draw(Assets.CTTexture, position, new Rectangle(0, 0, 32, 32), Color.White, 1.57f + rotation,
                    new Vector2(16, 16),
                    1f, SpriteEffects.None, 0);

                sb.DrawString(Assets.DefaultFont, UserName, new Vector2(position.X, position.Y - 50),
                    CurrentTeam == ServerClientInterface.Team.CounterTerrorist ? Color.Blue : Color.Red);

                CurrentWeapon.Draw(sb);
            }
        }

        public void Fire()
        {
            CurrentWeapon.FireWeapon();
        }

        public void SwitchWeapon()
        {

        }

        public void Plant()
        {
            
        }

        public void Defuse()
        {
            
        }

        /// <summary>
        /// Used by the server
        /// </summary>
        /// <param name="weapon"></param>
        public void SetCurrentWeapon(WeaponData.Weapon weapon)
        {
            CurrentWeapon = new ClientWeapon(weapon, this, Assets);
        }

        /// <summary>
        /// If the player was dead last round, give them default weapons specific
        /// to their team
        /// </summary>
        public void ResetWeapons()
        {
            switch (CurrentTeam)
            {
                case ServerClientInterface.Team.CounterTerrorist:
                    SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.Usp, this, Assets);
                    break;
                case ServerClientInterface.Team.Terrorist:
                    SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.Glock, this, Assets);
                    break;
            }
            PrimaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this, Assets);
            Knife = new ClientWeapon(WeaponData.Weapon.Knife, this, Assets);
            CurrentWeapon = Knife;
        }

        public void Move(byte direction)
        {
            float moveX = 0;
            float moveY = 0;
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    moveY -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    moveY += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    moveX -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    moveX += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    moveX -= ServerClientInterface.MOVEMENT_SPEED;
                    moveY -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    moveX += ServerClientInterface.MOVEMENT_SPEED;
                    moveY -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    moveX += ServerClientInterface.MOVEMENT_SPEED;
                    moveY += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    moveX -= ServerClientInterface.MOVEMENT_SPEED;
                    moveY += ServerClientInterface.MOVEMENT_SPEED;
                    break;
            }

            Vector2 normalized = Vector2.Normalize(new Vector2(moveX, moveY));
            position += normalized * ServerClientInterface.MOVEMENT_SPEED;
        }

        public void SetHealth(int health)
        {
            Health = health;
        }

        public void SetArmor(int armor)
        {
            Armor = armor;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetRotation(float rotation)
        {
            this.rotation = rotation;
        }

        public void SetTeam(byte team)
        {
            CurrentTeam = ServerClientInterface.ByteToTeam(team);
        }

        public void SetState(byte state)
        {
            State = ServerClientInterface.ByteToState(state);
        }

        public void Respawn(Vector2 location)
        {
            position = location;
            State = ServerClientInterface.PlayerState.Alive;
            CurrentWeapon = Knife;
            ResetWeapons();
        }

        public void Damage(int health, int armor)
        {
            Health -= health;
            Armor -= armor;
        }
    }
}