using System;
using Microsoft.Xna.Framework;

namespace sandbox4
{
    public class MonoSpawnTimer
    {
        private float _timePassed;
        private float _timeToSpawn;
        private bool _initialized;

        public MonoSpawnTimer()
        {
            _initialized = false;
        }

        /// <summary>
        /// Initialize a single repeating timer.
        /// </summary>
        /// <param name="spawnTime"></param>
        public void Initialize(float spawnTime)
        {
            _timePassed = 0f;
            _timeToSpawn = spawnTime;
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
