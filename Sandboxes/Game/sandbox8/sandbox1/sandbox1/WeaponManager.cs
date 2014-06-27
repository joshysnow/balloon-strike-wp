using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox8
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

        private LinkedList<Weapon> _currentWeapons;
        private List<Reticle> _garbageReticles;

        public WeaponManager()
        {
            _currentWeapons = new LinkedList<Weapon>();
            _currentWeapons.AddFirst(WeaponFactory.CreateDefault());

            _garbageReticles = new List<Reticle>();
        }

        public void Reset()
        {
            _currentWeapons.Clear();
            _currentWeapons.AddFirst(WeaponFactory.CreateDefault());
        }

        public void ProcessPowerup(PowerupType powerupType)
        {
            switch (powerupType)
            {
                case PowerupType.Shell:
                    AddWeapon(WeaponType.Shotgun);
                    break;
                case PowerupType.Missile:
                    AddWeapon(WeaponType.RocketLauncher);
                    break;
                default:
                    return;
            }            
        }

        public void UpdateInput(Vector2 lastPosition)
        {
            CurrentWeapon.UpdateInput(lastPosition);
        }

        public void Update(GameTime gameTime)
        {
            UpdateWeapons(gameTime);
            UpdateGarbage(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentWeapon.Draw(spriteBatch);

            foreach (Reticle reticle in _garbageReticles)
            {
                reticle.Draw(spriteBatch);
            }

#warning TODO: Show the player the amount of weapons per type they have.
        }

        private void AddWeapon(WeaponType newWeaponType)
        {
            bool inserted = false;
            LinkedListNode<Weapon> currentNode = _currentWeapons.First;
            Weapon newWeapon = WeaponFactory.CreateFromType(newWeaponType);

            do
            {
                if (currentNode.Value.IsBetterThan(newWeaponType) == false)
                {
                    _currentWeapons.AddBefore(currentNode, newWeapon);

                    // Put copy of current weapon into garbage? Need to still show the current reticle fading out.
                    _garbageReticles.Add(currentNode.Value.Reticle);

                    inserted = true;
                }

                currentNode = currentNode.Next;
            }
            while (!inserted && currentNode != null);
        }

        private void UpdateWeapons(GameTime gameTime)
        {
            LinkedListNode<Weapon> currentNode = _currentWeapons.First;
            Weapon currentWeapon;

            do
            {
                currentWeapon = currentNode.Value;

                if ((currentWeapon.HasAmmo == false) && (currentWeapon.Type != WeaponType.Finger))
                {
                    _currentWeapons.Remove(currentNode);
                    _garbageReticles.Add(currentWeapon.Reticle);
                }
                else
                {
                    currentWeapon.Update(gameTime);
                }

                currentNode = currentNode.Next;
            }
            while (currentNode != null);
        }

        private void UpdateGarbage(GameTime gameTime)
        {
            Reticle currentReticle;
            int i = 0;

            while(i < _garbageReticles.Count)
            {
                currentReticle = _garbageReticles[i];

                if (currentReticle.Visible == false)
                {
                    _garbageReticles.RemoveAt(i);
                }
                else
                {
                    currentReticle.Update(gameTime);
                    i++;
                }
            }
        }
    }
}
