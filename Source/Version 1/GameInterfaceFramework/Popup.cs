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

        protected Vector2 GetTransitionPosition(Vector2 endPosition)
        {
            Vector2 newPosition = endPosition; // Copy end values.
            float newValue;

            if (Transition.State == TransitionState.TransitionOn)
            {

                GetHorizontalTransitionPosition(newPosition, out newValue);
                newPosition.X = newValue;
            }

            if (Transition.State == TransitionState.TransitionOff)
            {
                GetVerticalTransitionPosition(newPosition, out newValue);
                newPosition.Y = newValue;
            }

            return newPosition;
        }

        private void GetHorizontalTransitionPosition(Vector2 position, out float positionX)
        {
            float offset = (float)Math.Pow(TransitionAlpha, 2);

            // Calculate horizontal difference for foreground.
            int totalWidth = (int)(ForegroundPosition.X + ForegroundSize.X);
            float distanceToTravel = totalWidth * offset;

            // Calculate the position should be.
            int startPosition = (int)(position.X - totalWidth);
            positionX = startPosition + distanceToTravel;
        }

        private void GetVerticalTransitionPosition(Vector2 position, out float positionY)
        {
            float offset = (float)Math.Pow(TransitionAlpha, 2);

            // Any position negated the total length doesn't alter the relative positions of all components.
            int totalHeight = (int)(ForegroundPosition.Y + ForegroundSize.Y + BUTTON_VERTICAL_SPACING + _buttonHeight);
            float distanceToTravel = totalHeight * offset;

            int endPosition = (int)(position.Y - totalHeight);
            positionY = endPosition + distanceToTravel;
        }

        private Vector2 GetMessagePositon()
        {
            return GetTransitionPosition(_messagePosition);
        }

        private Vector2 GetForegroundPosition()
        {
            return GetTransitionPosition(ForegroundPosition);
        }
    }
}
