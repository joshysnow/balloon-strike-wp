using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum WeaponType : byte
    {
        Finger          = 0x01,
        Shotgun         = 0x02,
        Bazooka         = 0x04
    }

    public class Weapon 
    {
        public Crosshair Crosshair
        {
            get { return _crossHair; }
        }

        public WeaponType Type
        {
            get { return _model.Type; }
        }

        public byte Damage
        {
            get { return _model.Damage; }
        }

        public byte Ammo
        {
            get { return _ammo; }
            set { _ammo = value; }
        }

        public bool HasAmmo
        {
            get { return _ammo > 0; }
        }

        private WeaponModel _model;
        private Crosshair _crossHair;
        private byte _ammo;

        public Weapon(WeaponModel wpModel, Crosshair xHair)
        {
            _model = wpModel;
            _crossHair = xHair;
        }

        public bool IsBetterThan(WeaponType type)
        {
            return (byte)Type > (byte)type;
        }

        public static bool IsBetterThan(Weapon left, Weapon right)
        {
            return (byte)left.Type > (byte)right.Type;
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
