using System;
using Microsoft.Xna.Framework;

namespace sandbox3
{
    public class SpawnTimer
    {
        private float _timePassed;
        private float _timeToSpawn;
        private bool _initialized;

        public SpawnTimer()
        {
            _initialized = false;
        }

        public void Initialize(float ms)
        {
            _timePassed = 0f;
            _timeToSpawn = ms;
            _initialized = true;
        }

        public bool Update(GameTime gameTime) 
        {
            if (!_initialized)
            {
                return false;
            }

            _timePassed += gameTime.ElapsedGameTime.Milliseconds;

            if (_timePassed >= _timeToSpawn)
            {
                _timePassed %= _timeToSpawn;
                return true;
            }

            return false;
        }
    }
}
