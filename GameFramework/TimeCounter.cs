using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using GameCore;

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

        /// <summary>
        /// Private constructor to rehydrate the object with.
        /// </summary>
        /// <param name="count">Number of ms passed.</param>
        /// <param name="limit">Number of ms to pass to elapse.</param>
        /// <param name="elapsed">Limit has been reached.</param>
        private TimeCounter(float count, float limit, bool elapsed)
        {
            _count = count;
            _limit = limit;
            _elapsed = elapsed;
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

        public XElement Dehydrate()
        {
            XElement xCounter = new XElement("TimeCounter",
                new XAttribute("Elapsed", _elapsed),
                new XAttribute("Count", _count),
                new XAttribute("Limit", _limit)
                );

            return xCounter;
        }

        public static TimeCounter Rehydrate(XElement counterElement)
        {
            TimeCounter counter = null;

            if (counterElement.CompareName("TimeCounter"))
            {
                bool elapsed = bool.Parse(counterElement.Attribute("Elapsed").Value);
                float count = float.Parse(counterElement.Attribute("Count").Value);
                float limit = float.Parse(counterElement.Attribute("Limit").Value);

                counter = new TimeCounter(count, limit, elapsed);
            }

            return counter;
        }

        public void Reset()
        {
            _count = 0;
            _elapsed = false;
        }
    }
}
