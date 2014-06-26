using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox7
{
    public class WeaponManager
    {
        public Weapon CurrentWeapon
        {
            get
            {
                return _currentWeapons.First.Value;
            }
        }

        public WeaponType WeaponType
        {
            get
            {
                return _currentWeapon.Type;
            }
        }

        private Weapon _currentWeapon;
        private Weapon _previousWeapon;

        private LinkedList<Weapon> _currentWeapons;
        private List<Weapon> _garbage;

        public WeaponManager()
        {

            _currentWeapon = WeaponFactory.CreateDefault();

            _currentWeapons = new LinkedList<Weapon>();
            _currentWeapons.AddFirst(_currentWeapon);

            _garbage = new List<Weapon>();
        }

        public void Reset()
        {
            _currentWeapon = WeaponFactory.CreateDefault();
            _currentWeapons.Clear();
            _currentWeapons.AddFirst(_currentWeapon);
        }

        public void ProcessNewWeapon(WeaponType newWeaponType)
        {
            if (newWeaponType == _currentWeapon.Type)
            {
                _currentWeapon.Resupply();
            }
            else
            {
                ChangeWeapon(newWeaponType);
            }

            AddWeapon(newWeaponType);
        }

        public void UpdateInput(Vector2 lastPosition)
        {
            _currentWeapon.UpdateInput(lastPosition);
            CurrentWeapon.UpdateInput(lastPosition);
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

            UpdateWeapons(gameTime);
            UpdateGarbage(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentWeapon.Draw(spriteBatch);
            CurrentWeapon.Draw(spriteBatch);

#warning TODO: Show the player the amount of weapons per type they have.
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

        private void AddWeapon(WeaponType type)
        {
            bool inserted = false;
            LinkedListNode<Weapon> currentNode = _currentWeapons.First;
            Weapon newWeapon = WeaponFactory.CreateFromType(type);

            do
            {
                if (currentNode.Value.BetterThan(type) == false)
                {
                    _currentWeapons.AddBefore(currentNode, newWeapon);
                    inserted = true;
                }
            }
            while (!inserted);
        }

        private void UpdateWeapons(GameTime gameTime)
        {
            LinkedListNode<Weapon> currentNode = _currentWeapons.First;

            do
            {
                if (currentNode.Value.HasAmmo == false)
                {
                    _currentWeapons.Remove(currentNode);
                    _garbage.Add(currentNode.Value);
                }
                else
                {
                    currentNode.Value.Update(gameTime);
                }
            }
            while (currentNode != null);
        }

        private void UpdateGarbage(GameTime gameTime)
        {
            foreach (Weapon weapon in _garbage)
            {
                if (weapon.Visible == false)
                {
                    _garbage.Remove(weapon);
                }
                else
                {
                    weapon.Update(gameTime);
                }
            }
        }
    }
}
