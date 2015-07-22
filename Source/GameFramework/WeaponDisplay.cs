using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public class WeaponDisplay
    {
        private Transition _transition;
        private string _currentName;
        private string _nextName;

        public WeaponDisplay(string initialName)
        {
            _transition = new Transition();
            _transition.TransitionOnTime = TimeSpan.FromSeconds(1.5);
            _transition.TransitionOffTime = TimeSpan.FromSeconds(1.5);
            _transition.Invoked = true; // Wait for a new weapon before transitioning off.

            _currentName = initialName;
        }

        public void WeaponChange(string newWeaponName)
        {
            _transition.State = TransitionState.TransitionOff;
            _nextName = newWeaponName;
        }

        public void Update(GameTime gameTime)
        {
            _transition.Update(gameTime);

            if (_transition.State == TransitionState.Hidden)
            {
                _currentName = _nextName;
                _transition.State = TransitionState.TransitionOn;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteFont font = ResourceManager.Resources.GetFont("debug");
            Vector2 size = font.MeasureString(_currentName);
            Vector2 position = new Vector2() { 
                X = (240 - (size.X / 2)), 
                Y = ((800 - size.Y) - 10) };            

            spriteBatch.DrawString(font, _currentName, position, Color.Black * _transition.TransitionAlpha);
        }
    }
}
