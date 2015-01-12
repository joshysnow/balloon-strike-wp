using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class AchievementsView : View
    {
        private SpriteFont _font;

        public AchievementsView()
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(1);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(1);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _font = ResourceManager.Resources.GetFont("your_score");
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            GraphicsDevice graphics = ViewManager.GraphicsDevice;

            string text = "COMING SOON";
            Vector2 size = _font.MeasureString(text);

            spriteBatch.Begin();
            spriteBatch.DrawString(_font, text, new Vector2((graphics.Viewport.Width - size.X) / 2, (graphics.Viewport.Height - size.Y) / 2), Color.Black * TransitionAlpha);
            spriteBatch.End();
        }
    }
}
