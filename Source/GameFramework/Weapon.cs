using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameCore.Physics.Shapes;

namespace GameFramework
{
    public enum WeaponType : byte
    {
        Tap     = 0x01,
        Shotgun = 0x02,
        Bazooka = 0x04
    }

    public class Weapon 
    {
        public Circle Circle
        {
            get { return _collisionShape; }
        }

        public WeaponType Type
        {
            get { return _model.Type; }
        }

        public byte Damage
        {
            get { return _model.Damage; }
        }

        public float Delta
        {
            get { return _delta; }
            set { _delta = value; }
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

        public bool Visible
        {
            get { return _delta < 1; }
        }

        private WeaponModel _model;
        private Circle _collisionShape;
        private TimeSpan _fadeTime;
        private Vector2 _origin;
        private float _delta;
        private byte _ammo;

        public Weapon(WeaponModel model, ref TimeSpan fadeTime)
        {
            _model = model;
            _fadeTime = fadeTime;
            _delta = 1;

            _collisionShape = new Circle() { Radius = _model.CrosshairTexture.Width / 2 };
        }

        public static bool IsBetterThan(Weapon left, Weapon right)
        {
            return (byte)left.Type > (byte)right.Type;
        }

        public bool IsBetterThan(WeaponType type)
        {
            return (byte)Type > (byte)type;
        }

        public void UpdateInput(Vector2 position)
        {
            if (HasAmmo)
            {
                _ammo--;
            }

            _collisionShape.Center.X = position.X;
            _collisionShape.Center.Y = position.Y;

            _origin.X = (position.X - _collisionShape.Radius);
            _origin.Y = (position.Y - _collisionShape.Radius);

            _delta = 0;
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
                spriteBatch.Draw(_model.CrosshairTexture, _origin, Color.White * alpha);
            }
        }
    }
}
