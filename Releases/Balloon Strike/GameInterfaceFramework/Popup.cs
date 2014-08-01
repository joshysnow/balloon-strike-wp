using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameInterfaceFramework
{
    public class Popup : MenuView
    {
        protected SpriteFont Font
        {
            get;
            private set;
        }

        protected Vector2 ForegroundPosition
        {
            get;
            private set;
        }

        protected Vector2 ForegroundSize
        {
            get;
            private set;
        }

        private Texture2D _foreground;
        private Texture2D _background;

        public Popup()
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(1);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            IsPopup = true;

            EnabledGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ResourceManager resources = ResourceManager.Resources;
                Font = resources.GetFont("popup_text");
                _foreground = resources.GetTexture("popup_foreground");
                _background = resources.GetTexture("blank");

                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                ForegroundPosition = new Vector2(((graphics.Viewport.Width - _foreground.Width) / 2), (graphics.Viewport.Height - _foreground.Height) / 2);
                ForegroundSize = new Vector2(_foreground.Width, _foreground.Height);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            GraphicsDevice graphics = ViewManager.GraphicsDevice;

            Vector2 tempPosition = new Vector2(ForegroundPosition.X, ForegroundPosition.Y);
            Vector2 size = ForegroundSize;
            TransitionPosition(ref size, ref tempPosition);

            spriteBatch.Begin();
            spriteBatch.Draw(_background, graphics.Viewport.Bounds, Color.Black * (TransitionAlpha / 1.5f));
            spriteBatch.Draw(_foreground, tempPosition, Color.White);
            spriteBatch.End();
        }

        protected virtual void TransitionPosition(ref Vector2 size, ref Vector2 position)
        {
            float offset = (float)Math.Pow(Transition.TransitionPosition, 2);

            if (Transition.State == TransitionState.TransitionOn)
                position.X -= offset * (position.X + size.X);

            if (Transition.State == TransitionState.TransitionOff)
                position.Y -= offset * (position.Y + size.Y);
        }
    }
}
