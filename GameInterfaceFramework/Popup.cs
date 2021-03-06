﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameInterfaceFramework
{
    public class Popup : MenuView
    {
        protected Vector2 ForegroundTitlePosition
        {
            get;
            private set;
        }

        protected Vector2 ForegroundTitleSize
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

        protected const int GUI_SPACING                 = 20; // Pixels.
        protected const int BUTTON_HORIZONTAL_SPACING   = 20; // Pixels.

        private SpriteFont _titleFont;
        private SpriteFont _messageFont;
        private Texture2D _foregroundTitle;
        private Texture2D _foreground;
        private Texture2D _background;
        private Vector2 _titlePosition;
        private Vector2 _titleSize;
        private Vector2 _messagePosition;
        private Vector2 _messageSize;
        private string _title;
        private string _message;
        private int _buttonHeight;
        private bool _clickable;

        public Popup(string title, string message)
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            IsPopup = true;
            _clickable = false;

            ViewGestures = GestureType.Tap;

            _title = title;
            _message = message;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                ResourceManager resources = ResourceManager.Resources;

                _titleFont = resources.GetFont("popup_title");
                _messageFont = resources.GetFont("popup_message");
                _foregroundTitle = resources.GetTexture("popup_title_foreground");
                _foreground = resources.GetTexture("popup_foreground");
                _background = resources.GetTexture("blank");

                _buttonHeight = (int)_menuButtons.First().Size.Y;
                int totalHeight = _foregroundTitle.Height + GUI_SPACING + _foreground.Height + GUI_SPACING + _buttonHeight;

                ForegroundTitlePosition = new Vector2(((graphics.Viewport.Width - _foreground.Width) / 2), (graphics.Viewport.Height - totalHeight) / 2);
                ForegroundTitleSize = new Vector2(_foregroundTitle.Width, _foregroundTitle.Height);

                ForegroundPosition = new Vector2(((graphics.Viewport.Width - _foreground.Width) / 2),
                    (ForegroundTitlePosition.Y + _foregroundTitle.Height + GUI_SPACING));
                ForegroundSize = new Vector2(_foreground.Width, _foreground.Height);

                _titleSize = _titleFont.MeasureString(_title);
                _titlePosition = new Vector2(
                    ((graphics.Viewport.Width - _titleSize.X) / 2),
                    (ForegroundTitlePosition.Y + (ForegroundTitleSize.Y / 2)) - (_titleSize.Y / 2)
                );

                _messageSize = _messageFont.MeasureString(_message);
                _messagePosition = new Vector2(
                    ((graphics.Viewport.Width - _messageSize.X) / 2),                       // X.
                    (ForegroundPosition.Y + (ForegroundSize.Y / 2)) - (_messageSize.Y / 2)  // Y.
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

            Vector2 foreTitlePosition = GetForegroundTitlePosition();
            Vector2 forePosition = GetForegroundPosition();
            Vector2 titlePosition = GetTitlePosition();
            Vector2 messagePosition = GetMessagePositon();

            spriteBatch.Begin();
            spriteBatch.Draw(_background, graphics.Viewport.Bounds, Color.Black * (TransitionAlpha / 1.25f));
            spriteBatch.Draw(_foregroundTitle, foreTitlePosition, Color.White * TransitionAlpha);
            spriteBatch.Draw(_foreground, forePosition, Color.White * TransitionAlpha);
            spriteBatch.DrawString(_titleFont, _title, titlePosition, Color.White * TransitionAlpha);
            spriteBatch.DrawString(_messageFont, _message, messagePosition, Color.White * TransitionAlpha);
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
            int totalHeight = (int)(ForegroundPosition.Y + ForegroundSize.Y + GUI_SPACING + _buttonHeight);
            float distanceToTravel = totalHeight * offset;

            int endPosition = (int)(position.Y - totalHeight);
            positionY = endPosition + distanceToTravel;
        }

        private Vector2 GetForegroundTitlePosition()
        {
            return GetTransitionPosition(ForegroundTitlePosition);
        }

        private Vector2 GetForegroundPosition()
        {
            return GetTransitionPosition(ForegroundPosition);
        }

        private Vector2 GetTitlePosition()
        {
            return GetTransitionPosition(_titlePosition);
        }

        private Vector2 GetMessagePositon()
        {
            return GetTransitionPosition(_messagePosition);
        }
    }
}
