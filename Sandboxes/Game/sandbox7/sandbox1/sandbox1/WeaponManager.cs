using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox7
{
    public class WeaponManager
    {
        public WeaponType WeaponType
        {
            get
            {
                return _currentWeapon.Type;
            }
        }

        private Weapon _currentWeapon;
        private Weapon _previousWeapon;

        public WeaponManager()
        {
            ChangeWeapon(WeaponType.Finger);
        }

        public void Reset()
        {
            ChangeWeapon(WeaponType.Finger);
        }

        public void ProcessAmmunition(WeaponType ammoType)
        {
            if (ammoType == _currentWeapon.Type)
            {
                _currentWeapon.Resupply();
            }
            else
            {
                ChangeWeapon(ammoType);
            }
        }

        public void UpdateInput(Vector2 lastPosition)
        {
            _currentWeapon.UpdateInput(lastPosition);
        }

        public void Update(GameTime gameTime)
        {
            _currentWeapon.Update(gameTime);

            if (_currentWeapon.Type != WeaponType.Finger && !_currentWeapon.HasAmmo)
            {
                WeaponType newWeaponType = (WeaponType)(((int)_currentWeapon.Type) >> 0x01); // Half the value is the one before.
                ChangeWeapon(newWeaponType);
            }

            if (_previousWeapon != null)
            {
                if (_previousWeapon.Visible)
                {
                    _previousWeapon.Update(gameTime);
                }
                else
                {
                    _previousWeapon = null;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentWeapon.Draw(spriteBatch);
        }

        private void ChangeWeapon(WeaponType type)
        {
            _previousWeapon = _currentWeapon;

            switch (type)
            {              
                case WeaponType.Shotgun:
                    _currentWeapon = WeaponFactory.CreateShotgun();
                    break;
                case WeaponType.RocketLauncher:
                    _currentWeapon = WeaponFactory.CreateRocketLauncher();
                    break;
                case WeaponType.Finger:
                default:
                    _currentWeapon = WeaponFactory.CreateDefault();
                    break;
            }
        }
    }
}
