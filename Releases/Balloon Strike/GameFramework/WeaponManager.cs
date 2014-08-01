using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameFramework
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
        private List<Crosshair> _garbageReticles;

        public WeaponManager()
        {
            _currentWeapons = new LinkedList<Weapon>();
            _currentWeapons.AddFirst(WeaponFactory.CreateDefault());

            _garbageReticles = new List<Crosshair>();
        }

        public void Reset()
        {
            _currentWeapons.Clear();
            _currentWeapons.AddFirst(WeaponFactory.CreateDefault());
        }

        public void ApplyPowerup(PowerupType powerupType)
        {
            switch (powerupType)
            {
                case PowerupType.Shell:
                    AddWeapon(WeaponType.Shotgun);
                    break;
                case PowerupType.Rocket:
                    AddWeapon(WeaponType.Bazooka);
                    break;
                default:
                    return;
            }            
        }

        public void UpdateInput(GestureSample[] gestures)
        {
            Weapon currentWeapon;

            foreach (GestureSample gesture in gestures)
            {
                currentWeapon = CurrentWeapon;
                currentWeapon.UpdateInput(gesture.Position);

                if ((currentWeapon.Type != WeaponType.Finger) && (currentWeapon.HasAmmo == false))
                {
                    _currentWeapons.Remove(_currentWeapons.First);
                    _garbageReticles.Add(currentWeapon.Crosshair);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateWeapons(gameTime);
            UpdateGarbage(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentWeapon.Draw(spriteBatch);

            foreach (Crosshair reticle in _garbageReticles)
            {
                reticle.Draw(spriteBatch);
            }

            SpriteFont debugFont = ResourceManager.Resources.GetFont("debug");
            Vector2 position = new Vector2(0, (800 - debugFont.LineSpacing));
            Weapon current = CurrentWeapon;
            string text = "Total: " + _currentWeapons.Count + " Current: " + current.Type + " Ammo: " + current.Ammo;
            spriteBatch.DrawString(debugFont, text, position, Color.Purple);
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

                    // Put copy of current weapon into garbage? Need to still show the current crosshair fading out.
                    _garbageReticles.Add(currentNode.Value.Crosshair);

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

                if ((currentWeapon.Type != WeaponType.Finger) && (currentWeapon.HasAmmo == false))
                {
                    _currentWeapons.Remove(currentNode);
                    _garbageReticles.Add(currentWeapon.Crosshair);
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
            Crosshair currentReticle;
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
