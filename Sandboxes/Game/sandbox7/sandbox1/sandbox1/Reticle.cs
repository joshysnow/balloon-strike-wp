using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox7
{
    public class Reticle
    {
        public bool Visible
        {
            get
            {
                return _delta < 1;
            }
        }

        private Texture2D _reticleTexture;
        private Vector2 _position;
        private TimeSpan _fadeTime;
        private float _delta;

        public Reticle(TimeSpan fadeTime, Texture2D reticleTexture)
        {
            _fadeTime = fadeTime;
            _reticleTexture = reticleTexture;
            _delta = 1;

        }

        public void UpdatePosition(Vector2 newposition)
        {
            _position.X = newposition.X - (_reticleTexture.Width / 2);
            _position.Y = newposition.Y - (_reticleTexture.Height / 2);

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
                spriteBatch.Draw(_reticleTexture, _position, Color.White * alpha);
            }
        }
    }
}
