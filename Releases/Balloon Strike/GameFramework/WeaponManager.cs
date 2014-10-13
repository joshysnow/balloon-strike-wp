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
                return _weaponsInventory.First.Value;
            }
        }

        private LinkedList<Weapon> _weaponsInventory;
        private List<Crosshair> _garbageReticles;
        private WeaponDisplay _display;

        public WeaponManager()
        {
            _weaponsInventory = new LinkedList<Weapon>();
            _weaponsInventory.AddFirst(WeaponFactory.CreateDefault());
            _garbageReticles = new List<Crosshair>();

            _display = new WeaponDisplay(CurrentWeapon.Type.ToString());
        }

#warning NOT CALLED
        public void Reset()
        {
            _weaponsInventory.Clear();
            _weaponsInventory.AddFirst(WeaponFactory.CreateDefault());
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
                    _weaponsInventory.Remove(_weaponsInventory.First);
                    _garbageReticles.Add(currentWeapon.Crosshair);
                    _display.WeaponChange(CurrentWeapon.Type.ToString());
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

            _display.Draw(spriteBatch);

            /*SpriteFont debugFont = ResourceManager.Resources.GetFont("debug");
            Vector2 position = new Vector2(0, (800 - debugFont.LineSpacing));
            Weapon current = CurrentWeapon;
            string text = "Total: " + _weaponsInventory.Count + " Current: " + current.Type + " Ammo: " + current.Ammo;
            spriteBatch.DrawString(debugFont, text, position, Color.Purple); **/
        }

        private void AddWeapon(WeaponType newWeaponType)
        {
            bool inserted = false;
            LinkedListNode<Weapon> currentNode = _weaponsInventory.First;
            Weapon newWeapon = WeaponFactory.CreateFromType(newWeaponType);

#warning Needs improvement, new weapon is best scenario
            byte count = 0;

            do
            {
                if (currentNode.Value.IsBetterThan(newWeaponType) == false)
                {
                    _weaponsInventory.AddBefore(currentNode, newWeapon);

                    // Put copy of current weapon into garbage? Need to still show the current crosshair fading out.
                    _garbageReticles.Add(currentNode.Value.Crosshair);

                    inserted = true;

                    if(count == 0)
                        _display.WeaponChange(CurrentWeapon.Type.ToString());
                }

                currentNode = currentNode.Next;
                count++;
            }
            while (!inserted && currentNode != null);
        }

        private void UpdateWeapons(GameTime gameTime)
        {
            LinkedListNode<Weapon> currentNode = _weaponsInventory.First;
            Weapon currentWeapon;

            do
            {
                currentWeapon = currentNode.Value;

                if ((currentWeapon.Type != WeaponType.Finger) && (currentWeapon.HasAmmo == false))
                {
                    _weaponsInventory.Remove(currentNode);
                    _garbageReticles.Add(currentWeapon.Crosshair);
                    _display.WeaponChange(CurrentWeapon.Type.ToString());
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
