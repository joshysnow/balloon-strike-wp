using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    public class Pulse
    {
        public float Position
        {
            get;
            private set;
        }

        private bool _increase;
        private float _time;
        private float _duration;

        public Pulse(TimeSpan duration)
        {
            _duration = (float)duration.TotalMilliseconds / 2f;
            Position = 0;
            _increase = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_increase)
                _time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                _time -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            _time = MathHelper.Clamp(_time, 0, _duration);

            if (_time == 0f)
                _increase = true;

            if (_time == _duration)
                _increase = false;

            Position = _time / _duration;
        }
    }
}
