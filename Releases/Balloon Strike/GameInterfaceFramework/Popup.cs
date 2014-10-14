using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameInterfaceFramework
{
    public class Popup : MenuView
    {
        protected const int BUTTON_VERTICAL_SPACING = 20; // Pixels.

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
        private string _message;
        private bool _clickable;

        public Popup(string message)
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(1);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(1);
            IsPopup = true;
            _clickable = false;

            ViewGestures = GestureType.Tap;

            _message = message;
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

        public override void HandlePlayerInput(ControlsState controls)
        {
            if(_clickable)
                base.HandlePlayerInput(controls);
        }

        protected override void HandleBackButtonPressed()
        {
            Exit();
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (Transition.State == TransitionState.Active)
                _clickable = true;
            else
                _clickable = false;

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            GraphicsDevice graphics = ViewManager.GraphicsDevice;

            Vector2 messageSize = Font.MeasureString(_message);
            Vector2 messagePosition = new Vector2((graphics.Viewport.Width - messageSize.X) / 2, (graphics.Viewport.Height - messageSize.Y) / 2);
            TransitionPosition(ref messageSize, ref messagePosition);

            Vector2 forePosition = new Vector2(ForegroundPosition.X, ForegroundPosition.Y);
            Vector2 size = ForegroundSize;
            TransitionPosition(ref size, ref forePosition);

            spriteBatch.Begin();
            spriteBatch.Draw(_background, graphics.Viewport.Bounds, Color.Black * (TransitionAlpha / 1.5f));
            spriteBatch.Draw(_foreground, forePosition, Color.White);
            spriteBatch.DrawString(Font, _message, messagePosition, Color.White);
            spriteBatch.End();
        }

        protected virtual void TransitionPosition(ref Vector2 size, ref Vector2 position)
        {
            float offset = (float)Math.Pow(TransitionAlpha, 2);

            if (Transition.State == TransitionState.TransitionOn)
            {
                // Mirror flip copy position on the Y axis.
                float start = (Vector2.Zero - (position + size)).X;
                float distance = start - position.X;
                distance *= -1;

                position.X = start + (distance * offset);
            }

            if (Transition.State == TransitionState.TransitionOff)
            {
                int height = ViewManager.GraphicsDevice.Viewport.Height;

                float end = (0 - (height - position.Y));
                float distance = end - position.Y;
                distance *= -1;

                position.Y = end + (distance * offset);
            }
        }
    }
}
