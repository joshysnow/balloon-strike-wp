using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox9
{
    public class Crosshair
    {
        public bool Visible
        {
            get
            {
                return _delta < 1;
            }
        }

        public int Radius
        {
            get
            {
                return _crosshairTexture.Width / 2;
            }
        }

        private Texture2D _crosshairTexture;
        private Vector2 _position;
        private TimeSpan _fadeTime;
        private float _delta;

        public Crosshair(TimeSpan fadeTime, Texture2D crosshair)
        {
            _fadeTime = fadeTime;
            _crosshairTexture = crosshair;
            _delta = 1;

        }

        public void UpdatePosition(Vector2 newposition)
        {
            _position.X = newposition.X - (_crosshairTexture.Width / 2);
            _position.Y = newposition.Y - (_crosshairTexture.Height / 2);

            _delta = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                _delta += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / _fadeTime.TotalMilliseconds);
                _delta = MathHelper.Clamp(_delta, 0, 1);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                float alpha = (1 - _delta);
                spriteBatch.Draw(_crosshairTexture, _position, Color.White * alpha);
            }
        }
    }
}
