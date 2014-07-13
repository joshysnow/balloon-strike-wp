using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    public delegate void ElapsedHandler(SimpleTimer timer);

    public class SimpleTimer
    {
        public event ElapsedHandler Elapsed;

        protected float _elapseTime;
        private float _timePassed;

        /// <summary>
        /// Initialize a repeating timer.
        /// </summary>
        /// <param name="elapseTime">Time to tick over.</param>
        public SimpleTimer(float elapseTime)
        {
            _timePassed = 0f;
            _elapseTime = elapseTime;
        }

        public virtual bool Update(GameTime gameTime) 
        {
            _timePassed += gameTime.ElapsedGameTime.Milliseconds;

            if (_timePassed >= _elapseTime)
            {
                _timePassed %= _elapseTime;
                RaiseElapsed();

                return true;
            }

            return false;
        }

        private void RaiseElapsed()
        {
            if (Elapsed != null)
            {
                Elapsed(this);
            }
        }
    }
}
