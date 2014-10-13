using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public class WeaponDisplay
    {
        private Transition _transitionPosition;
        private string _weaponName;

        public WeaponDisplay(string initialName)
        {
            _transitionPosition = new Transition();
            _transitionPosition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionPosition.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            _transitionPosition.Invoked = true; // Wait for a new weapon before transitioning off.

            _weaponName = initialName;
        }

        public void WeaponChange(string newWeaponName)
        {
            _transitionPosition.State = TransitionState.TransitionOff;
            _weaponName = newWeaponName;
        }

        public void Update(GameTime gameTime)
        {
            _transitionPosition.Update(gameTime);

            if (_transitionPosition.State == TransitionState.Hidden)
                _transitionPosition.State = TransitionState.TransitionOn;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteFont font = ResourceManager.Resources.GetFont("debug");
            Vector2 size = font.MeasureString(_weaponName);
            Vector2 position = new Vector2() { 
                X = (240 - (size.X / 2)), 
                Y = ((800 - size.Y) - 10) };            

            spriteBatch.DrawString(font, _weaponName, position, Color.Black);
        }
    }
}
