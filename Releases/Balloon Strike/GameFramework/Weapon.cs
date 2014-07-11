using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public Crosshair Crosshair
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
        private byte _maxAmmo;

        public Weapon(WeaponType type)
        {
            Type = type;
            TimeSpan fadeTime = TimeSpan.FromSeconds(1.5);

            switch (type)
            {
                default:
                case WeaponType.Finger:
                    {
                        _ammo = 0;
                        _maxAmmo = 0;
                        Damage = 1;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Manager.GetTexture("xhair_finger"));
                    }
                    break;
                case WeaponType.Shotgun:
                    {
                        _ammo = 6;
                        _maxAmmo = 6;
                        Damage = 2;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Manager.GetTexture("xhair_shotgun"));
                    }
                    break;
                case WeaponType.Bazooka:
                    {
                        _ammo = 2;
                        _maxAmmo = 2;
                        Damage = 3;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Manager.GetTexture("xhair_bazooka"));
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

        public void Resupply()
        {
            _ammo = _maxAmmo;
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
