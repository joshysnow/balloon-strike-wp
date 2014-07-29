using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class SplashView : View
    {
        private Texture2D _splashTexture;
        private SimpleTimer _activeTimer;    

        public SplashView()
        {
            Transition.TransitionOff = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOn = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _activeTimer = new SimpleTimer(2000);
                _splashTexture = ResourceManager.Manager.GetTexture("splash");
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (State == TransitionState.Active)
            {
                if (_activeTimer.Update(gameTime))
                {
#warning TODO: Introduce parent counting here, so when a view decides to disappear it can take the whole chain or just itself from the view manager.
                    LoadView.Load(ViewManager, 1, new BackgroundView(), new MainMenuView(), new MessagePopup("Welcome to the Alpha!"));
                }
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
