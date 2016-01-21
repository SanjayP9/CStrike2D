// Author: Mark Voong
// File Name: ServerWeapon.cs
// Project Name: Global Offensive
// Creation Date: Jan 15th, 2016
// Modified Date: Jan 19th, 2016
// Description: Serverside version of the weapon class.
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    public class ServerWeapon
    {
        /// <summary>
        /// The owner of the weapon
        /// </summary>
        public ServerPlayer Owner { get; private set; }

        /// <summary>
        /// If the weapon was fired
        /// </summary>
        public bool Fired { get; private set; }

        /// <summary>
        /// Position of the weapon
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Unique identifier for the weapon
        /// </summary>
        public short EntityIdentifier { get; private set; }

        /// <summary>
        /// Reload time of the weapon
        /// </summary>
        public float ReloadTime { get; private set; }

        /// <summary>
        /// Fire rate of the weapon
        /// </summary>
        public float FireRate { get; private set; }

        /// <summary>
        /// Type of weapon
        /// </summary>
        public WeaponData.Weapon Weapon { get; private set; }

        /// <summary>
        /// Creates a new weapon
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="owner"></param>
        public ServerWeapon(WeaponData.Weapon weapon, ServerPlayer owner)
        {
            Owner = owner;
            Weapon = weapon;
            Fired = false;
        }

        /// <summary>
        /// Fires the weapon
        /// </summary>
        public void FireWeapon()
        {
            if (!Fired)
            {
                Fired = true;
            }
        }

        /// <summary>
        /// Update logic for the weapon
        /// </summary>
        public void Update()
        {
            // If the weapon hasn't been dropped on the floor
            if (Owner != null)
            {
                // The owner is carrying the weapon, set their position
                // to the owner
                Position = Owner.Position;

                // If the weapon was fired
                if (Fired)
                {
                    // Subtract a timer starting from its fire rate in
                    // milliseconds until 0, allowing the weapon to be fired again
                    if (FireRate >= 0f)
                    {
                        FireRate -= 0.1666f;
                    }
                    else
                    {
                        Fired = false;
                    }
                }
            }
        }

        /// <summary>
        /// Drops the weapon
        /// </summary>
        public void Drop()
        {
            Owner = null;
        }

        /// <summary>
        /// Allows a player to pickup the weapon
        /// </summary>
        /// <param name="owner"></param>
        public void Pickup(ServerPlayer owner)
        {
            Owner = owner;
        }
    }
}
