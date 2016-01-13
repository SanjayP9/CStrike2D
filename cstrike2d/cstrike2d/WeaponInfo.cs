using System;

namespace CStrike2D
{
    public static class WeaponInfo
    {

        public enum Weapon
        {
            Weapon_AK47,
            Weapon_AWP,
            Weapon_Knife,
        }

        public static readonly string[] WeaponStrings = Enum.GetNames(typeof (Weapon));
        public static readonly Weapon[] WeaponEnums = (Weapon[])Enum.GetValues(typeof(Weapon));
        
        public enum WeaponType
        {
            Primary,
            Secondary,
            Knife,
            Grenade,
            Bomb
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
                case Weapon.Weapon_AWP:
                    return WeaponType.Primary;
                default:
                    throw new ArgumentOutOfRangeException("weapon", weapon, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weaponID"></param>
        /// <returns></returns>
        public static Weapon GetWeapon(short weaponID)
        {
            if (weaponID >= 100)
            {
                return WeaponEnums[weaponID - 100];
            }
            throw new NotImplementedException("Weapon does not exist");
        }
    }
}