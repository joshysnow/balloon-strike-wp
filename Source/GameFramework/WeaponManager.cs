using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameFramework
{
    public class WeaponManager : Serializable
    {
        private const string STORAGE_FILE_NAME = "WEAPON_MANAGER.xml";

        public Weapon CurrentWeapon
        {
            get
            {
                return _weaponsInventory.First.Value;
            }
        }

        private LinkedList<Weapon> _weaponsInventory;   // Stores picked up weapons.
        private List<Crosshair> _garbageReticles;       // Holds all cross-hairs to display until unvisible.
        private WeaponDisplay _display;                 // Used to display current weapon type on screen.

        public WeaponManager()
        {
            _weaponsInventory = new LinkedList<Weapon>();
            _garbageReticles = new List<Crosshair>();
        }

        public void Initialize()
        {
            _weaponsInventory.AddFirst(WeaponFactory.CreateDefault());
            _display = new WeaponDisplay(CurrentWeapon.Type.ToString());
        }

        public void Activate(bool instancePreserved)
        {
            
        }

        public void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("WeaponManager");

                XElement weaponsRoot = new XElement("Weapons");

                XElement crosshairsRoot = new XElement("CrossHairs");

                root.Add(weaponsRoot);
                root.Add(crosshairsRoot);
                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
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
                    break;
            }            
        }

        public void UpdateInput(GestureSample[] gestures)
        {
            Weapon currentWeapon;

            foreach (GestureSample gesture in gestures)
            {
                currentWeapon = CurrentWeapon;
                currentWeapon.UpdateInput(gesture.Position);

                // If the current weapon has run out of ammo, change it, as long as it isn't the default.
                if ((currentWeapon.Type != WeaponType.Finger) && (currentWeapon.HasAmmo == false))
                {
                    _weaponsInventory.Remove(_weaponsInventory.First);
                    _garbageReticles.Add(currentWeapon.Crosshair);

                    // Change to display the new current weapon (new head on the list)
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
        }

        private void AddWeapon(WeaponType newWeaponType)
        {            
            LinkedListNode<Weapon> weapon = _weaponsInventory.First;
            Weapon newWeapon = WeaponFactory.CreateFromType(newWeaponType);

            bool topWeapon = true;

            do
            {
                // When a weapon in the inventory is no longer better than the weapon to add.
                if (weapon.Value.IsBetterThan(newWeaponType) == false)
                {
                    // Add new weapon ahead of one that it is better than
                    _weaponsInventory.AddBefore(weapon, newWeapon);

                    // Put copy of current weapon into garbage? Need to still show the current crosshair fading out.
                    _garbageReticles.Add(weapon.Value.Crosshair);

                    // If weapon is better than currently used weapon, change to this weapon!
                    if (topWeapon)
                        _display.WeaponChange(CurrentWeapon.Type.ToString());

                    break;
                }

                topWeapon = false;
                weapon = weapon.Next;
            }
            while (weapon != null);
        }

        private void UpdateWeapons(GameTime gameTime)
        {
            LinkedListNode<Weapon> currentNode = _weaponsInventory.First;
            Weapon currentWeapon;

            while (currentNode != null)
            {
                currentWeapon = currentNode.Value;
                currentWeapon.Update(gameTime);

                currentNode = currentNode.Next;
            }
        }

        private void UpdateGarbage(GameTime gameTime)
        {
            Crosshair currentReticle;
            int i = 0;

            while(i < _garbageReticles.Count)
            {
                currentReticle = _garbageReticles[i];

                // If the cross-hair is no longer visible on the screen.
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
