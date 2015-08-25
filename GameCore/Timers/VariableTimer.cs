using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace GameCore.Timers
{
    public class VariableTimer
    {
        private SimpleTimer _timer;
        private float _modifier;
        private float _bounds;

        /// <summary>
        /// Takes a percentage modifier that manipulates the next time to spawn up to the bounds,
        /// can also fire off immediately by setting elapse to true.
        /// </summary>
        /// <param name="elapseTime">Time to elapse in ms.</param>
        /// <param name="modifier">Percentage to apply to elapse time for every elapse.</param>
        /// <param name="bounds">Minimum time to spawn in ms.</param>
        /// <param name="elapse">If true, timer will fire immediately.</param>
        public VariableTimer(float elapseTime, float modifier, float bounds, bool elapse = false)
        {
            _modifier = modifier;
            _bounds = bounds;

            _timer = new SimpleTimer(elapseTime, elapse);
        }

        private VariableTimer(SimpleTimer timer, float modifier, float bounds)
        {
            _timer = timer;
            _modifier = modifier;
            _bounds = bounds;
        }

        public bool Update(GameTime gameTime) 
        {
            bool elapsed = _timer.Update(gameTime);

            if (elapsed)
            {
                float elapseTime = _timer.ElapseTime;

                if (elapseTime > _bounds)
                {
                    float newTime = (elapseTime * _modifier);

                    // Make sure the modified limit doesn't exceed the bounds.
                    if (newTime <= _bounds)
                        elapseTime = _bounds;
                    else 
                        elapseTime = newTime;
                }
            }

            return elapsed;
        }

        public XElement Dehydrate()
        {
            XElement xVarTimer = new XElement("VariableTimer",
                new XAttribute("Modifier", _modifier),
                new XAttribute("Bounds", _bounds)
                );

            XElement xTimer = _timer.Dehydrate();
            xVarTimer.Add(xTimer);

            return xVarTimer;
        }

        public static VariableTimer Rehydrate(XElement timerElement)
        {
            VariableTimer varTimer = null;

            if (timerElement.Name.Equals("VariableTimer"))
            {
                float modifier = float.Parse(timerElement.Attribute("Modifier").Value);
                float bounds = float.Parse(timerElement.Attribute("Bounds").Value);

                SimpleTimer timer = SimpleTimer.Rehydrate(timerElement.Element("Timer"));

                varTimer = new VariableTimer(timer, modifier, bounds);
            }

            return varTimer;
        }
    }
}