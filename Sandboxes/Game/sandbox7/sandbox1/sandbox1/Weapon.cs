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

#warning TODO: Change visiblie property to a calculation involving delta time i.e. when the time remaining is zero, then it is no longer visible.
        public bool Visible
        {
            get;
            private set;
        }

        public bool HasAmmo
        {
            get
            {
                return _ammo > 0;
            }
        }

        private Vector2 _position;
        private Texture2D _reticle;
        private byte _ammo;
        private byte _maxAmmo;

        public Weapon(WeaponType type)
        {
            Type = type;
            Visible = false;

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
            if (!Visible)
            {
                Visible = true;  
            }

            if (HasAmmo)
            {
                _ammo--;
            }

            // TODO: Reset reticle opacity to 100%.
            _position = lastPosition;
        }

        public void Update(GameTime gameTime)
        {
            if (Visible)
            {

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(_reticle, _position, Color.White);
            }
        }
    }
}
