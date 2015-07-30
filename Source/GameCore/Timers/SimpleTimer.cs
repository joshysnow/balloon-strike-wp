using System;
using Microsoft.Xna.Framework;

namespace GameCore.Timers
{
    public delegate void ElapsedHandler(SimpleTimer timer);

    public class SimpleTimer
    {
        public event ElapsedHandler Elapsed;

        /// <summary>
        /// In milliseconds (ms).
        /// </summary>
        public float ElapseTime
        {
            get { return _elapseTime; }
        }

        /// <summary>
        /// In miliseconds (ms).
        /// </summary>
        public float TimePassed
        {
            get { return _timePassed; }
        }

        protected float _elapseTime;
        private float _timePassed;

        /// <summary>
        /// Initialize a repeating timer.
        /// </summary>
        /// <param name="elapseTime">Time to tick over in ms.</param>
        public SimpleTimer(float elapseTime)
        {
            _timePassed = 0f;
            _elapseTime = elapseTime;
        }

        /// <summary>
        /// Initialize a repeating timer with the option to elapse on first update.
        /// </summary>
        /// <param name="elapseTime">Time to tick over in ms.</param>
        public SimpleTimer(float elapseTime, bool elapse) : this(elapseTime)
        {
            if (elapse)
            {
                _timePassed = elapseTime;
            }
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
