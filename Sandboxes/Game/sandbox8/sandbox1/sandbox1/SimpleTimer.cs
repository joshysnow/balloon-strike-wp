using System;
using Microsoft.Xna.Framework;

namespace sandbox8
{
    public class SimpleTimer
    {
        private float _timePassed;
        private float _elapseTime;
        private bool _initialized;

        public SimpleTimer()
        {
            _initialized = false;
        }

        /// <summary>
        /// Initialize a single repeating timer.
        /// </summary>
        /// <param name="elapseTime"></param>
        public void Initialize(float elapseTime)
        {
            _timePassed = 0f;
            _elapseTime = elapseTime;
            _initialized = true;
        }

        public bool Update(GameTime gameTime) 
        {
            if (!_initialized)
            {
                return false;
            }

            _timePassed += gameTime.ElapsedGameTime.Milliseconds;

            if (_timePassed >= _elapseTime)
            {
                _timePassed %= _elapseTime;
                return true;
            }

            return false;
        }
    }
}
