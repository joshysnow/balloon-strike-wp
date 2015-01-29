using System;
using System.Linq;
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

        protected const int BUTTON_VERTICAL_SPACING = 20; // Pixels.

        private Texture2D _foreground;
        private Texture2D _background;
        private Vector2 _messagePosition;
        private Vector2 _messageSize;
        private string _message;
        private int _buttonHeight;
        private bool _clickable;

        public Popup(string message)
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(1);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(5);
            IsPopup = true;
            _clickable = false;

            ViewGestures = GestureType.Tap;

            _message = message;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                ResourceManager resources = ResourceManager.Resources;

                Font = resources.GetFont("popup_text");
                //_foreground = resources.GetTexture("popup_foreground");
                _foreground = resources.GetTexture("bg_test");
                _background = resources.GetTexture("blank");

                _buttonHeight = (int)_menuButtons.First().Size.Y;
                int totalHeight = _foreground.Height + BUTTON_VERTICAL_SPACING + _buttonHeight;

                ForegroundPosition = new Vector2(((graphics.Viewport.Width - _foreground.Width) / 2), (graphics.Viewport.Height - totalHeight) / 2);
                ForegroundSize = new Vector2(_foreground.Width, _foreground.Height);

                _messageSize = Font.MeasureString(_message);
                _messagePosition = new Vector2(
                    ((graphics.Viewport.Width - _messageSize.X) / 2),
                    ((graphics.Viewport.Height - BUTTON_VERTICAL_SPACING - _buttonHeight - _messageSize.Y) / 2)
                );
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

            Vector2 messagePosition = GetMessagePositon();
            Vector2 forePosition = GetForegroundPosition();

            spriteBatch.Begin();
            spriteBatch.Draw(_background, graphics.Viewport.Bounds, Color.Black * (TransitionAlpha / 1.25f));
            spriteBatch.Draw(_foreground, forePosition, Color.White * TransitionAlpha);
            spriteBatch.DrawString(Font, _message, messagePosition, Color.White * TransitionAlpha);
            spriteBatch.End();

            base.Draw(gameTime);
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

        protected void GetHorizontalTransitionPosition(Vector2 position, out float transPosition)
        {
            // Calculate horizontal difference for foreground.
            int foregroundDiff = (int)(ForegroundSize.X + ForegroundPosition.X);
            float distanceToTravel = foregroundDiff * TransitionAlpha;

            // Calculate where the position is on the screen.
            int startPosition = (int)(position.X - foregroundDiff);
            transPosition = startPosition + distanceToTravel;
        }

        private Vector2 GetMessagePositon()
        {
            Vector2 messagePosition = _messagePosition;

            if (Transition.State == TransitionState.TransitionOn)
            {
                float newValue;
                GetHorizontalTransitionPosition(messagePosition, out newValue);
                messagePosition.X = newValue;
            }
            else
            {
                TransitionPosition(ref _messageSize, ref messagePosition);
            }

            return messagePosition;
        }

        private Vector2 GetForegroundPosition()
        {
            Vector2 foregroundPosition = ForegroundPosition;

            if (Transition.State == TransitionState.TransitionOn)
            {
                float newValue;
                GetHorizontalTransitionPosition(foregroundPosition, out newValue);
                foregroundPosition.X = newValue;
            }
            else
            {
                Vector2 size = ForegroundSize;
                TransitionPosition(ref size, ref foregroundPosition);
            }

            return foregroundPosition;
        }
    }
}
