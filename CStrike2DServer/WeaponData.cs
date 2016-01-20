using System;

namespace CStrike2DServer
{
    public static class WeaponData
    {
        // Byte equivalents of weapons in the game
        public const byte WEAPON_KNIFE = 0;
        public const byte WEAPON_AK47 = 1;
        public const byte WEAPON_GLOCK = 2;
        public const byte WEAPON_AWP = 3;
        public const byte WEAPON_USP = 4;
        public const byte WEAPON_M4A1 = 5;
        public const byte WEAPON_NONE = 255;

        public static readonly string[] WeaponStrings = Enum.GetNames(typeof(Weapon));
        public static readonly Weapon[] WeaponEnums = (Weapon[])Enum.GetValues(typeof(Weapon));
        

        /// <summary>
        /// Different types of weapons
        /// </summary>
        public enum Weapon
        {
            Knife,
            Ak47,
            Glock,
            Awp,
            Usp,
            M4A1,
            None
        }

        public static float FireRate(Weapon weapon)
        {
            switch (weapon)
            {
                case Weapon.Knife:
                    return 1.0f;
                case Weapon.Ak47:
                    return 0.06f;
                case Weapon.Glock:
                    return 0.5f;
                case Weapon.Awp:
                    return 1.0f;
                case Weapon.Usp:
                    return 0.4f;
                case Weapon.M4A1:
                    return 0.06f;
                case Weapon.None:
                    return 1.0f;
                default:
                    throw new ArgumentOutOfRangeException("weapon");
            }
        }
        

        /// <summary>
        /// Returns the byte equivalent of a weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public static byte WeaponToByte(Weapon weapon)
        {
            switch (weapon)
            {
                case Weapon.Knife:
                    return WEAPON_KNIFE;
                case Weapon.Ak47:
                    return WEAPON_AK47;
                case Weapon.Glock:
                    return WEAPON_GLOCK;
                case Weapon.Awp:
                    return WEAPON_AWP;
                case Weapon.Usp:
                    return WEAPON_USP;
                case Weapon.M4A1:
                    return WEAPON_M4A1;
                case Weapon.None:
                    return WEAPON_NONE;
                default:
                    throw new NotImplementedException("Weapon does not exist.");
            }
        }

        /// <summary>
        /// Returns the enum equivalent of a weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public static Weapon ByteToWeapon(byte weapon)
        {
            switch (weapon)
            {
                case WEAPON_KNIFE:
                    return Weapon.Knife;
                case WEAPON_AK47:
                    return Weapon.Ak47;
                case WEAPON_GLOCK:
                    return Weapon.Glock;
                case WEAPON_AWP:
                    return Weapon.Awp;
                case WEAPON_USP:
                    return Weapon.Usp;
                case WEAPON_M4A1:
                    return Weapon.M4A1;
                case WEAPON_NONE:
                    return Weapon.None;
                default:
                    throw new NotImplementedException("Weapon does not exist.");
            }
        }
    }
}