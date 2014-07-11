using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class AboutView : View
    {
        private SpriteFont _font;
        private string _message;

        public AboutView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _font = ResourceManager.Manager.GetFont("debug");

                const byte NAME = 0;
                const byte VERSION = 1;
                string[] info = Assembly.GetExecutingAssembly().FullName.Split(',');
                _message = info[NAME].Trim() + "\n" + info[VERSION].Trim();
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(_font, _message, Vector2.Zero, Color.SpringGreen * TransitionAlpha);
            spriteBatch.End();
        }
    }
}
