using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox7
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

        public bool Visible
        {
            get
            {
                return _delta < 1;
            }
        }

        public bool HasAmmo
        {
            get
            {
                return _ammo > 0;
            }
        }

        private Vector2 _inputPosition;
        private Texture2D _reticle;
        private TimeSpan _fadeTime;
        private float _delta;
        private byte _ammo;
        private byte _maxAmmo;


        public Weapon(WeaponType type)
        {
            Type = type;
            _delta = 1;
            _fadeTime = TimeSpan.FromSeconds(1.5);

            switch (type)
            {
                default:
                case WeaponType.Finger:
                    _ammo = 0;
                    _maxAmmo = 0;
                    _reticle = ResourceManager.Manager.GetTexture("reticle_finger");
                    break;
                case WeaponType.Shotgun:
                    _ammo = 6;
                    _maxAmmo = 6;
                    _reticle = ResourceManager.Manager.GetTexture("reticle_shotgun");
                    break;
                case WeaponType.RocketLauncher:
                    _ammo = 2;
                    _maxAmmo = 2;
                    _reticle = ResourceManager.Manager.GetTexture("reticle_rocketlauncher");
                    break;
            }
        }

        public void Resupply()
        {
            _ammo = _maxAmmo;
        }

        public void UpdateInput(Vector2 lastPosition)
        {
            if (HasAmmo)
            {
                _ammo--;
            }

            // Reset delta so reticle is now fully visible again.
            _delta = 0;

            _inputPosition.X = lastPosition.X - (_reticle.Width / 2);
            _inputPosition.Y = lastPosition.Y - (_reticle.Height / 2);
        }

        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                _delta += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / _fadeTime.TotalMilliseconds);
                _delta = MathHelper.Clamp(_delta, 0, 1);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                float alpha = (1 - _delta);
                spriteBatch.Draw(_reticle, _inputPosition, Color.White * alpha);
            }
        }
    }
}
