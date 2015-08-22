using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    /// <summary>
    /// This class is to be used to simply keep track of time.
    /// No thrills or spills, it can be used as a delegate for more complex
    /// objects like Timers, Triggers, Spawners etc.
    /// </summary>
    public class TimeCounter
    {
        public bool Elapsed
        {
            get { return _elapsed; }
        }

        public float Count
        {
            get { return _count; }
        }

        private float _count;
        private float _limit;
        private bool _elapsed;

        /// <summary>
        /// Even if limit is zero it will not set elapsed until
        /// update is called at least once.
        /// </summary>
        /// <param name="limit">Time to elapse.</param>
        public TimeCounter(TimeSpan limit)
        {
            _count = 0;
            _limit = (float)limit.TotalMilliseconds;
            _elapsed = false;
        }

        public void Update(GameTime gameTime)
        {
            if (_elapsed == false)
            {
                _count += gameTime.ElapsedGameTime.Milliseconds;

                if (_count >= _limit)
                {
                    _elapsed = true;
                }
            }
        }

        public void Reset()
        {
            _count = 0;
            _elapsed = false;
        }
    }
}
