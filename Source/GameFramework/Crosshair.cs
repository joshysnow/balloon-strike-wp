using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFramework
{
    public class Crosshair
    {
        public GameCore.Physics.Shapes.Circle Circle
        {
            get { return _model.Circle; }
        }

        public float Delta
        {
            get { return _delta; }
        }

        public bool Visible
        {
            get { return _delta < 1; }
        }

        private CrosshairModel _model;
        private Vector2 _origin;
        private TimeSpan _fadeTime;
        private float _delta;

        public Crosshair(CrosshairModel model, ref TimeSpan fadeTime)
        {
            _delta = 1;
            _model = model;
            _fadeTime = fadeTime;
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            Circle.Center.X = newPosition.X;
            Circle.Center.Y = newPosition.Y;

            _origin.X = (newPosition.X - _model.Circle.Radius);
            _origin.Y = (newPosition.Y - _model.Circle.Radius);

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
                spriteBatch.Draw(_model.Texture, _origin, Color.White * alpha);
            }
        }
    }
}
