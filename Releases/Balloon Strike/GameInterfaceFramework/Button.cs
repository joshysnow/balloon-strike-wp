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

                _lowerRight.X = (_origin.X + _unselectedTexture.Width);
                _lowerRight.Y = (_origin.Y + _unselectedTexture.Height);
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(_lowerRight.X - Origin.X, _lowerRight.Y - Origin.Y);
            }
        }

        public bool Selected
        {
            get;
            private set;
        }

        private Texture2D _unselectedTexture;
        private Texture2D _selectedTexture;
        private Vector2 _origin;
        private Vector2 _lowerRight;

        public Button(Texture2D unselected, Texture2D selected)
        {
            _unselectedTexture = unselected;
            _selectedTexture = selected;
            Selected = false;
        }

        public bool HandleTap(Vector2 position)
        {
            bool tapped = false;

            if ((position.X >= _origin.X) && (position.X <= _lowerRight.X) &&
                 (position.Y >= _origin.Y) && (position.Y <= _lowerRight.Y))
            {
                RaiseButtonTapped();
                tapped = true;
                Selected = !Selected;
            }

            return tapped;
        }

        public void Draw(View parentView)
        {
            SpriteBatch spriteBatch = parentView.ViewManager.SpriteBatch;
            spriteBatch.Draw((Selected ? _selectedTexture : _unselectedTexture), _origin, (Color.White * parentView.TransitionAlpha));
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
