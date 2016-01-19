using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PlayerState State { get; private set; }
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

        public enum PlayerState
        {
            Dead,
            Alive
        }

        public ClientPlayer(string username, short identifier, Assets assets)
            : base(assets)
        {
            UserName = username;
            Identifier = identifier;
            CurrentTeam = ServerClientInterface.Team.Spectator;
            State = PlayerState.Dead;
            Health = 100;
            Armor = 0;
            PrimaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this);
            SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this);
            Knife = new ClientWeapon(WeaponData.Weapon.Knife, this);
            CurrentWeapon = Knife;
        }

        public override void Update(float gameTime)
        {
            CurrentWeapon.Update(gameTime);
            PrimaryWeapon.Update(gameTime);
            SecondaryWeapon.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (CurrentTeam != ServerClientInterface.Team.Spectator && State != PlayerState.Dead)
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
            CurrentWeapon = new ClientWeapon(weapon, this);
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
                    SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.Usp, this);
                    break;
                case ServerClientInterface.Team.Terrorist:
                    SecondaryWeapon = new ClientWeapon(WeaponData.Weapon.Glock, this);
                    break;
            }
            PrimaryWeapon = new ClientWeapon(WeaponData.Weapon.None, this);
            Knife = new ClientWeapon(WeaponData.Weapon.Knife, this);
            CurrentWeapon = Knife;
        }

        public void Move(byte direction)
        {
            switch (direction)
            {
                case ServerClientInterface.MOVE_UP:
                    position.Y -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWN:
                    position.Y += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_LEFT:
                    position.X -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_RIGHT:
                    position.X += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPLEFT:
                    position.X -= ServerClientInterface.MOVEMENT_SPEED;
                    position.Y -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_UPRIGHT:
                    position.X += ServerClientInterface.MOVEMENT_SPEED;
                    position.Y -= ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNRIGHT:
                    position.X += ServerClientInterface.MOVEMENT_SPEED;
                    position.Y += ServerClientInterface.MOVEMENT_SPEED;
                    break;
                case ServerClientInterface.MOVE_DOWNLEFT:
                    position.X -= ServerClientInterface.MOVEMENT_SPEED;
                    position.Y += ServerClientInterface.MOVEMENT_SPEED;
                    break;
            }
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

        public void Respawn(Vector2 location)
        {
            position = location;
            State = PlayerState.Alive;
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