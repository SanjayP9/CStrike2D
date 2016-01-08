using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class Weapon : Entity
    {
        public WeaponInfo.Weapon WeaponName { get; private set; }

        public Texture2D WeaponTexture { get; private set; }

        public Weapon(byte weaponID, Texture2D weaponTexture, Assets assets) : base(assets)
        {

        }

        public override int DrawOrder { get; protected set; }
        public override Vector2 Position { get; protected set; }
        public override Rectangle Dimensions { get; protected set; }

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

        public void Fire()
        {
            if (!Firing)
            {
                Firing = true;
                cooldownTimer = FireRate();
            }    
        }

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

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Assets.);
        }
    }
}
