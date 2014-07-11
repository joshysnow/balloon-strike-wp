using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameInterfaceFramework
{
    public delegate void ButtonTappedHandler(Button button);

    public class Button
    {
        public event ButtonTappedHandler Tapped;

        private Texture2D _texture;
        private Vector2 _upperLeft;
        private Vector2 _lowerRight;

        public Button(Texture2D texture, Vector2 topLeft)
        {
            _texture = texture;
            _upperLeft = topLeft;
            _lowerRight = new Vector2((topLeft.X + texture.Width), (topLeft.Y + texture.Height));
        }

        public bool HandleTap(Vector2 position)
        {
            bool tapped = false;

            if ((position.X >= _upperLeft.X) && (position.X <= _lowerRight.X) &&
                 (position.Y >= _upperLeft.Y) && (position.Y <= _lowerRight.Y))
            {
                RaiseButtonTapped();
                tapped = true;
            }

            return tapped;
        }

        public void Draw(View parentView)
        {
            SpriteBatch spriteBatch = parentView.ViewManager.SpriteBatch;
            spriteBatch.Draw(_texture, _upperLeft, (Color.White * parentView.TransitionAlpha));
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
