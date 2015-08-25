using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace GameCore.Timers
{
    public delegate void ElapsedHandler(SimpleTimer timer);

    public class SimpleTimer
    {
        public event ElapsedHandler Elapsed;

        public float ElapseTime
        {
            get { return _elapseTime; }
        }

        private float _elapseTime;
        private float _timePassed;

        /// <summary>
        /// Initialize a repeating timer with the option to elapse on first update.
        /// </summary>
        /// <param name="elapseTime">Time to tick over in ms.</param>
        /// <param name="elapse">Fire on first update call or not.</param>
        public SimpleTimer(float elapseTime, bool elapse = false)
        {
            _elapseTime = elapseTime;

            if (elapse)
            {
                _timePassed = elapseTime;
            }
        }

        private SimpleTimer(float timePassed, float elapseTime)
        {
            _timePassed = timePassed;
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

        public XElement Dehydrate()
        {
            XElement xTimer = new XElement("Timer",
                new XAttribute("ElapseTime", _elapseTime),
                new XAttribute("TimePassed", _timePassed)
                );

            return xTimer;
        }

        public static SimpleTimer Rehydrate(XElement timerElement)
        {
            SimpleTimer timer = null;

            if (timerElement.Name.Equals("Timer"))
            {
                float elapseTime = float.Parse(timerElement.Attribute("ElapseTime").Value);
                float timePassed = float.Parse(timerElement.Attribute("TimePassed").Value);

                timer = new SimpleTimer(timePassed, elapseTime);
            }

            return timer;
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