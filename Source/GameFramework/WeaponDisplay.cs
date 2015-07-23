using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public class WeaponDisplay
    {
        private const string SPACE = " ";
        private const string INFINITY = "\u221E";
        private const string MULTIPLY = "x";

        private Transition _transition;
        private string _currentName;
        private string _currentAmmo;
        private string _nextName;
        private string _nextAmmo;

        public WeaponDisplay(WeaponManager parent)
        {
            _transition = new Transition();
            _transition.TransitionOnTime = TimeSpan.FromSeconds(0.25);
            _transition.TransitionOffTime = TimeSpan.FromSeconds(0.25);
            _transition.Invoked = true; // Wait for a new weapon before transitioning off.

            parent.WeaponUpdate += WeaponUpdateHandler;
        }

        public void Initialize(Weapon initial)
        {
            SetName(initial);
            SetAmmo(initial);

            _transition.State = TransitionState.Hidden;
        }

        public void Update(GameTime gameTime)
        {
            _transition.Update(gameTime);

            if (_transition.State == TransitionState.Hidden)
            {
                _currentName = _nextName;
                _currentAmmo = _nextAmmo;
                _transition.State = TransitionState.TransitionOn;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string output = _currentName + SPACE + _currentAmmo;
            SpriteFont font = ResourceManager.Resources.GetFont("debug");
            Vector2 size = font.MeasureString(output);
            Vector2 position = new Vector2() { 
                X = (240 - (size.X / 2)), 
                Y = ((800 - size.Y) - 10) };            

            spriteBatch.DrawString(font, output, position, Color.Black * _transition.TransitionAlpha);
        }

        private void SetName(Weapon weapon)
        {
            _nextName = weapon.Type.ToString();
        }

        private void SetAmmo(Weapon weapon)
        {
            if (weapon.Type == WeaponType.Finger)
                _nextAmmo = INFINITY;
            else
                _nextAmmo = MULTIPLY + weapon.Ammo.ToString();
        }

        private void WeaponUpdateHandler(WeaponUpdateEvent evt, object data)
        {
            switch (evt)
            {
                case WeaponUpdateEvent.WEAPON_CHANGE:
                    WeaponChangeHandler(data);
                    break;
                case WeaponUpdateEvent.WEAPON_FIRED:
                    WeaponAmmoFiredHandler(data);
                    break;
                default:
                    break;
            }
        }

        private void WeaponChangeHandler(object data)
        {
            // Set the display name and update ammo counter
            Weapon weapon = (Weapon)data;
            SetName(weapon);
            SetAmmo(weapon);

            _transition.State = TransitionState.TransitionOff;
        }

        private void WeaponAmmoFiredHandler(object data)
        {            
            Weapon weapon = (Weapon)data;

            // Update ammo counter
            SetAmmo(weapon);

            if (_currentAmmo != _nextAmmo)
                _transition.State = TransitionState.TransitionOff;
        }
    }
}
