using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox9
{
    public enum WeaponType : byte
    {
        Finger          = 0x01,
        Shotgun         = 0x02,
        RocketLauncher  = 0x04
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
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Manager.GetTexture("reticle_finger"));
                    }
                    break;
                case WeaponType.Shotgun:
                    {
                        _ammo = 6;
                        _maxAmmo = 6;
                        Damage = 2;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Manager.GetTexture("reticle_shotgun"));
                    }
                    break;
                case WeaponType.RocketLauncher:
                    {
                        _ammo = 2;
                        _maxAmmo = 2;
                        Damage = 3;
                        Crosshair = new Crosshair(fadeTime, ResourceManager.Manager.GetTexture("reticle_rocketlauncher"));
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
