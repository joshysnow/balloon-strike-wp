using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameInterfaceFramework
{
    public delegate void ButtonTappedHandler(Button button);

    public class Button
    {
        public event ButtonTappedHandler Tapped;

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;

                _lowerRight.X = (_origin.X + _texture.Width);
                _lowerRight.Y = (_origin.Y + _texture.Height);
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(_lowerRight.X - Origin.X, _lowerRight.Y - Origin.Y);
            }
        }

        private Texture2D _texture;
        private Vector2 _origin;
        private Vector2 _lowerRight;

        public Button(Texture2D texture, Vector2 origin)
        {
            _texture = texture;
            Origin = origin;
        }

        public bool HandleTap(Vector2 position)
        {
            bool tapped = false;

            if ((position.X >= _origin.X) && (position.X <= _lowerRight.X) &&
                 (position.Y >= _origin.Y) && (position.Y <= _lowerRight.Y))
            {
                RaiseButtonTapped();
                tapped = true;
            }

            return tapped;
        }

        public void Draw(View parentView)
        {
            SpriteBatch spriteBatch = parentView.ViewManager.SpriteBatch;
            spriteBatch.Draw(_texture, _origin, (Color.White * parentView.TransitionAlpha));
        }

        private void RaiseButtonTapped()
        {
            if (Tapped != null)
            {
                Tapped(this);
            }
        }
    }
}
