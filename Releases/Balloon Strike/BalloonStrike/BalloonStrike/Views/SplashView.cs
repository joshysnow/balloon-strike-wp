using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class SplashView : View
    {
        private Texture2D _splashTexture;   

        public SplashView()
        {
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.ActiveTime = TimeSpan.FromSeconds(2);
            Transition.Invoked = false;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _splashTexture = ResourceManager.Resources.GetTexture("splash");
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (State == TransitionState.Hidden)
            {
#warning TODO: Introduce parent counting here, so when a view decides to disappear it can take the whole chain or just itself from the view manager.
                LoadView.Load(ViewManager, 1, new BackgroundView(), new MainMenuView(), new MessagePopup("Welcome to the Alpha!"));
            }

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(_splashTexture, Vector2.Zero, Color.White * TransitionAlpha);
            spriteBatch.End();
        }
    }
}
