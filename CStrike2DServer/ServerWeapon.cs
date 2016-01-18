using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CStrike2D;
using Microsoft.Xna.Framework;

namespace CStrike2DServer
{
    public class ServerWeapon
    {
        public ServerPlayer Owner { get; private set; }

        public bool Fired { get; private set; }

        public Vector2 Position { get; private set; }

        public short EntityIdentifier { get; private set; }

        public float ReloadTime { get; private set; }
        public float FireRate { get; private set; }


        public WeaponData.Weapon Weapon { get; private set; }

        public ServerWeapon(WeaponData.Weapon weapon, ServerPlayer owner)
        {
            Owner = owner;
            Weapon = weapon;
        }

        public void Update()
        {
            if (Owner != null)
            {
                Position = Owner.Position;
            }
        }

        public void Drop()
        {
            Owner = null;
        }

        public void Pickup(ServerPlayer owner)
        {
            Owner = owner;
        }

        public void Update(float gameTime)
        {
            
        }
    }
}
