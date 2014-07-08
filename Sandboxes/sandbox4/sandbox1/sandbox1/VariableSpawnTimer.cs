using System;
using Microsoft.Xna.Framework;

namespace sandbox4
{
    public class VariableSpawnTimer
    {
        private float _timePassed;
        private float _spawnTime;
        private float _modifier;
        private float _bounds;
        private bool _initialized;

        public VariableSpawnTimer()
        {
            _initialized = false;
        }

        /// <summary>
        /// Takes a percentage modifier that manipulates the next time to spawn up to the bounds.
        /// </summary>
        /// <param name="spawnTime"></param>
        /// <param name="modifier"></param>
        /// <param name="bounds"></param>
        public void Initialize(float spawnTime, float modifier, float bounds)
        {
            _timePassed = 0f;
            _spawnTime = spawnTime;
            _modifier = modifier;
            _bounds = bounds;
            _initialized = true;
        }

        public bool Update(GameTime gameTime) 
        {
            if (!_initialized)
            {
                return false;
            }

            _timePassed += gameTime.ElapsedGameTime.Milliseconds;

            if (_timePassed >= _spawnTime)
            {
                _timePassed %= _spawnTime;

                if (_spawnTime < _bounds)
                {
                    if (_spawnTime * _modifier >= _bounds)
                    {
                        _spawnTime = _bounds;
                    }
                    else
                    {
                        _spawnTime *= _modifier;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
