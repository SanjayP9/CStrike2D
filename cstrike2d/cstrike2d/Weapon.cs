using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Weapon : Entity
    {
        public WeaponInfo.Weapon WeaponName { get; private set; }

        public Texture2D WeaponTexture { get; private set; }

        public Weapon(short entityID, short weaponID, Vector2 spawnPosition, Assets assets) : base(assets)
        {
            EntityID = entityID;
            WeaponName = WeaponInfo.GetWeapon(weaponID);
            Position = spawnPosition;
        }

        public override int DrawOrder { get; protected set; }
        public override Vector2 Position { get; protected set; }
        public override Rectangle Dimensions { get; protected set; }

        public float Rotation { get; private set; }

        public short EntityID { get; private set; }

        public bool Firing { get; private set; }

        private float cooldownTimer;

        /// <summary>
        /// Gets the firerate of this weapon
        /// </summary>
        /// <returns></returns>
        public float FireRate()
        {
            return WeaponInfo.GetFireRate(WeaponName);
        }

        /// <summary>
        /// Gets the type of weapon
        /// </summary>
        /// <returns></returns>
        public WeaponInfo.WeaponType WeaponType()
        {
            return WeaponInfo.GetWeaponType(WeaponName);
        }

        /// <summary>
        /// Changes the weapon to another weapon
        /// </summary>
        /// <param name="newWeapon"></param>
        public void SetWeaponName(WeaponInfo.Weapon newWeapon)
        {
            WeaponName = newWeapon;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Fire()
        {
            if (!Firing)
            {
                Firing = true;
                cooldownTimer = FireRate();
            }    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetDirection(float rotation)
        {
            Rotation = rotation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(float gameTime)
        {
            if (Firing)
            {
                if (cooldownTimer >= 0f)
                {
                    cooldownTimer -= gameTime;
                }
                else
                {
                    Firing = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Assets.GetWeaponTexture(WeaponName), Position, null, Color.White, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}