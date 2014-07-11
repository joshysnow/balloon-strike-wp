using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;
using GameInterfaceFramework;

namespace Balloonstrike.Views
{
    public class SplashView : View
    {
        private Texture2D _splashTexture;
        private SimpleTimer _activeTimer;    

        public SplashView()
        {
            _transitionOffTime = TimeSpan.FromSeconds(0.5);
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _activeTimer = new SimpleTimer();
                _activeTimer.Initialize(2000);

                _splashTexture = ResourceManager.Manager.GetTexture("splash");
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (State == ViewState.Active)
            {
                if (_activeTimer.Update(gameTime))
                {
                    Exit();
#warning TODO: Introduce parent counting here, so when a view decides to disappear it can take the whole chain or just itself from the view manager.
                    LoadView.Load(ViewManager, 1, new BackgroundView(), new MainMenuView());
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
