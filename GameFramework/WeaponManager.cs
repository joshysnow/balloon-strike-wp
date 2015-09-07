using System;
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
    public enum WeaponUpdateEvent : byte
    {
        WEAPON_CHANGE = 0x01,
        WEAPON_FIRED  = 0x02
    }

    public delegate void WeaponUpdate(WeaponUpdateEvent evt, object data);

    public class WeaponManager
    {
        private const string STORAGE_FILE_NAME = "WEAPON_MANAGER.xml";

        public event WeaponUpdate WeaponUpdate;

        public Weapon CurrentWeapon
        {
            get
            {
                return _inventory.First.Value;
            }
        }

        private LinkedList<Weapon> _inventory;      // Stores picked up weapons.
        private List<Weapon> _graveyard;            // Holds all weapons that have expended to display their crosshair until unvisible.
        private WeaponDisplay _display;             // Used to display current weapon type on screen.
        private WeaponFactory _weaponFactory;       // Constructs weapons efficiently for us.

        public WeaponManager()
        {
            _inventory = new LinkedList<Weapon>();
            _graveyard = new List<Weapon>();
            _weaponFactory = new WeaponFactory();
        }

        public void Activate(bool instancePreserved, bool newGame)
        {
            if (!instancePreserved)
            {
                // Load weapon resources
                _weaponFactory.Initialize();

                if (newGame)
                {
                    InitializeDefault();
                }
                else
                {
                    Rehydrate();
                }

                Weapon mainWeapon = CurrentWeapon;

                _display = new WeaponDisplay(this);
                _display.Initialize(mainWeapon);
            }
        }

        public void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("WeaponManager");

                XElement weaponsRoot = new XElement("Weapons");
                XElement weaponXML;
                Weapon currentWeapon;

                LinkedListNode<Weapon> weaponNode = _inventory.Last;

                // Store in reverse order so when the game rehydrates, the weapons are added
                // to the head of the list for a small performance increase
                while (weaponNode != null)
                {
                    currentWeapon = weaponNode.Value;

                    weaponXML = new XElement("Weapon",
                        new XAttribute("Type", currentWeapon.Type),
                        new XAttribute("Ammo", currentWeapon.Ammo),
                        new XAttribute("Delta", currentWeapon.Delta)
                        );

                    weaponsRoot.Add(weaponXML);
                    
                    weaponNode = weaponNode.Previous;
                }

                XElement graveyardRoot = new XElement("Graveyard");

                foreach (Weapon weapon in _graveyard)
                {
                    weaponXML = new XElement("Weapon",
                        new XAttribute("Type", weapon.Type),
                        new XAttribute("Delta", weapon.Delta)
                        );

                    graveyardRoot.Add(weaponXML);
                }

                root.Add(weaponsRoot);
                root.Add(graveyardRoot);
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
                if ((currentWeapon.Type != WeaponType.Tap) && (currentWeapon.HasAmmo == false))
                {
                    _inventory.Remove(_inventory.First);
                    _graveyard.Add(currentWeapon);

                    // Change to display the new current weapon (new head on the list)
                    RaiseWeaponUpdate(WeaponUpdateEvent.WEAPON_CHANGE, CurrentWeapon);
                }
                else
                {
                    RaiseWeaponUpdate(WeaponUpdateEvent.WEAPON_FIRED, currentWeapon);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateWeapons(gameTime);
            UpdateGraveyard(gameTime);

            _display.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentWeapon.Draw(spriteBatch);

            foreach (Weapon weapon in _graveyard)
            {
                weapon.Draw(spriteBatch);
            }

            _display.Draw(spriteBatch);
        }

        private void InitializeDefault()
        {
            Weapon defaultWeapon = _weaponFactory.MakeWeapon(WeaponType.Tap);
            _inventory.AddFirst(defaultWeapon);
        }

        private void Rehydrate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(STORAGE_FILE_NAME))
                {
                    // Rehydrate the weapons
                    using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);
                        XElement root = doc.Root;

                        // Use this to get the type when parsing WeaponType attribute from saved data
                        WeaponType temp = WeaponType.Tap;

                        foreach (XElement weaponXML in root.Element("Weapons").Elements())
                        {
                            // Extract saved data on weapon
                            WeaponType weaponType = (WeaponType)Enum.Parse(temp.GetType(), weaponXML.Attribute("Type").Value, false);
                            float delta = float.Parse(weaponXML.Attribute("Delta").Value);
                            byte ammo = Byte.Parse(weaponXML.Attribute("Ammo").Value);

                            // Rehydrate weapon
                            Weapon weapon = _weaponFactory.MakeWeapon(weaponType);
                            weapon.Ammo = ammo;
                            weapon.Delta = delta;

                            // Add to the head of weapons list
                            _inventory.AddFirst(new LinkedListNode<Weapon>(weapon));
                        }

                        foreach (XElement weaponXML in root.Element("Graveyard").Elements())
                        {
                            WeaponType weaponType = (WeaponType)Enum.Parse(temp.GetType(), weaponXML.Attribute("Type").Value, false);
                            float delta = float.Parse(weaponXML.Attribute("Delta").Value);

                            // Rehydrate weapon
                            Weapon weapon = _weaponFactory.MakeWeapon(weaponType);
                            weapon.Ammo = 0;
                            weapon.Delta = delta;

                            _graveyard.Add(weapon);
                        }
                    }

                    storage.DeleteFile(STORAGE_FILE_NAME);
                }
                else
                {
                    // Add the default weapon (act like this is a new game scenario)
                    InitializeDefault();
                }
            }
        }

        private void AddWeapon(WeaponType newWeaponType)
        {            
            LinkedListNode<Weapon> weapon = _inventory.First;
            Weapon newWeapon = _weaponFactory.MakeWeapon(newWeaponType);

            bool topWeapon = true;

            do
            {
                // When a weapon in the inventory is no longer better than the weapon to add.
                if (weapon.Value.IsBetterThan(newWeaponType) == false)
                {
                    // Add new weapon ahead of one that it is better than
                    _inventory.AddBefore(weapon, newWeapon);

                    // Put copy of current weapon into garbage. Need to still show the current crosshair fading out.
                    _graveyard.Add(weapon.Value);

                    // If weapon is better than currently used weapon, notify top weapon has changed!
                    if (topWeapon)
                        RaiseWeaponUpdate(WeaponUpdateEvent.WEAPON_CHANGE, newWeapon);

                    break;
                }

                topWeapon = false;
                weapon = weapon.Next;
            }
            while (weapon != null);
        }

        private void UpdateWeapons(GameTime gameTime)
        {
            // TODO: Is it not the top weapon that only needs updating?
            //CurrentWeapon.Update(gameTime);

            LinkedListNode<Weapon> currentNode = _inventory.First;
            Weapon currentWeapon;

            while (currentNode != null)
            {
                currentWeapon = currentNode.Value;
                currentWeapon.Update(gameTime);

                currentNode = currentNode.Next;
            }
        }

        private void UpdateGraveyard(GameTime gameTime)
        {
            Weapon currentWeapon;
            int i = 0;

            while(i < _graveyard.Count)
            {
                currentWeapon = _graveyard[i];

                // If the cross-hair is no longer visible on the screen.
                if (currentWeapon.Visible == false)
                {
                    _graveyard.RemoveAt(i);
                }
                else
                {
                    currentWeapon.Update(gameTime);
                    i++;
                }
            }
        }

        private void RaiseWeaponUpdate(WeaponUpdateEvent evt, object data)
        {
            if (WeaponUpdate != null)
            {
                WeaponUpdate(evt, data);
            }
        }
    }
}
