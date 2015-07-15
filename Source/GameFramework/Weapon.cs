using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum WeaponType : byte
    {
        Finger          = 0x01,
        Shotgun         = 0x02,
        Bazooka         = 0x04
    }

    public class Weapon 
    {
        public Crosshair Crosshair
        {
            get;
            private set;
        }

        public WeaponType Type
        {
            get;
            private set;
        }

        public byte Damage
        {
            get;
            private set;
        }

        public byte Ammo
        {
            get 
            { 
                return _ammo;
            }
        }

        public bool HasAmmo
        {
            get
            {
                return _ammo > 0;
            }
        }

        private byte _ammo;

        public Weapon(WeaponType type)
        {
            Type = type;
            TimeSpan fadeTime = TimeSpan.FromSeconds(1.5);

            // TODO: Need to change this into a Flyweight and Factory pattern set of objects
            // Flyweight to be able to store an instance of the weapon types but not store any position information
            // just use it as a reference.
            // Factory pattern to be able to construct and return these models which is useful when rehydrating, as
            // it would only need to store the type and ammo to rebuild the object as damage would be set by the
            // model (flyweight).

            switch (type)
            {
                default:
                case WeaponType.Finger:
                    {
                        _ammo = 0;
                        Damage = 1;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Resources.GetTexture("xhair_finger"));
                    }
                    break;
                case WeaponType.Shotgun:
                    {
                        _ammo = 6;
                        Damage = 2;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Resources.GetTexture("xhair_shotgun"));
                    }
                    break;
                case WeaponType.Bazooka:
                    {
                        _ammo = 2;
                        Damage = 3;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Resources.GetTexture("xhair_bazooka"));
                    }
                    break;
            }
        }

        public bool IsBetterThan(WeaponType type)
        {
            return (byte)Type > (byte)type;
        }

        public static bool IsBetterThan(Weapon left, Weapon right)
        {
            return (byte)left.Type > (byte)right.Type;
        }

        public void UpdateInput(Vector2 position)
        {
            if (HasAmmo)
            {
                _ammo--;
            }

            Crosshair.UpdatePosition(position);
        }

        public void Update(GameTime gameTime)
        {
            Crosshair.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Crosshair.Draw(spriteBatch);
        }
    }
}
