using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameInterfaceFramework;
using GameFramework;

namespace BalloonStrike.Views
{
    public class IntroPopup : View
    {
        private SpriteFont _font;
        private string _message;

        public IntroPopup()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _isPopup = true;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _font = ResourceManager.Manager.GetFont("debug");
                _message = "Welcome to the Alpha!";
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            Vector2 messageSize = _font.MeasureString(_message);
            Vector2 messagePosition = new Vector2((graphics.Viewport.Width - messageSize.X) / 2, graphics.Viewport.Height / 2);

            spriteBatch.Begin();
            spriteBatch.DrawString(_font, _message, messagePosition, Color.Black * TransitionAlpha);
            spriteBatch.End();
        }
    }
}
