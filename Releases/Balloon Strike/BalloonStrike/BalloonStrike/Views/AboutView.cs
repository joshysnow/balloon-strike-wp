using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class AboutView : View
    {
        private SpriteFont _font;
        private string _message;

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _font = ResourceManager.Manager.GetFont("debug");
                _message = Assembly.GetExecutingAssembly().FullName;
                //Assembly.GetExecutingAssembly();
                //_message = "Balloon Strike\nVersion: " + Assembly.GetExecutingAssembly().GetName().Version + "\nFoxCode Studios\nSupport: example@email.com";
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
            spriteBatch.DrawString(_font, _message, Vector2.Zero, Color.SpringGreen);
            spriteBatch.End();
        }
    }
}
