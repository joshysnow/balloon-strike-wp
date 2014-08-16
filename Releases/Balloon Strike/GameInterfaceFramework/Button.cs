using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameInterfaceFramework
{
    public delegate void ButtonTappedHandler(Button button);

    public class Button
    {
        public event ButtonTappedHandler Tapped;

        public Vector2 Position
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
                return new Vector2(_lowerRight.X - Position.X, _lowerRight.Y - Position.Y);
            }
        }

        public float Scalar
        {
            get;
            set;
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
            Scalar = 1f;
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
            spriteBatch.Draw((Selected ? _selectedTexture : _unselectedTexture), _origin, null, (Color.White * parentView.TransitionAlpha), 0f, Vector2.Zero, Scalar, SpriteEffects.None, 0f);
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
