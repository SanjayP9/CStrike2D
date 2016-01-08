using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public static class WeaponInfo
    {

        public enum Weapon
        {
            Weapon_AK47,
            Weapon_AWP
        }

        public enum WeaponType
        {
            Primary,
            Secondary
        }


        /// <summary>
        /// Gets the time in seconds between each shot for
        /// a given weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public static float GetFireRate(Weapon weapon)
        {
            switch (weapon)
            {
                case Weapon.Weapon_AK47:
                    return 0.1f;
                case Weapon.Weapon_AWP:
                    return 1.464f;
                default:
                    throw new ArgumentOutOfRangeException("weapon", weapon, null);
            }
        }

        /// <summary>
        /// Gets the weapon type of a given weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public static WeaponType GetWeaponType(Weapon weapon)
        {
            switch (weapon)
            {
                case Weapon.Weapon_AK47:
                    return WeaponType.Primary;
                case Weapon.Weapon_AWP:
                    return WeaponType.Primary;
                default:
                    throw new ArgumentOutOfRangeException("weapon", weapon, null);
            }
        }
    }
}
