using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace GameCore.Timers
{
    public class VariableTimer : SimpleTimer
    {
        private float _modifier;
        private float _bounds;

        /// <summary>
        /// Takes a percentage modifier that manipulates the next time to spawn up to the bounds.
        /// </summary>
        /// <param name="elapseTime">Time to elapse in ms.</param>
        /// <param name="modifier">Percentage to apply to elapse time for every elapse.</param>
        /// <param name="bounds">Minimum time to spawn.</param>
        public VariableTimer(float elapseTime, float modifier, float bounds)
            : base(elapseTime)
        {
            _modifier = modifier;
            _bounds = bounds;
        }

        /// <summary>
        /// Takes a percentage modifier that manipulates the next time to spawn up to the bounds,
        /// can also fire off immediately by setting elapse to true.
        /// </summary>
        /// <param name="elapseTime">Time to elapse in ms.</param>
        /// <param name="modifier">Percentage to apply to elapse time for every elapse.</param>
        /// <param name="bounds">Minimum time to spawn in ms.</param>
        /// <param name="elapse">If true, timer will fire immediately.</param>
        public VariableTimer(float elapseTime, float modifier, float bounds, bool elapse)
            : base(elapseTime, elapse)
        {
            _modifier = modifier;
            _bounds = bounds;
        }

        public override bool Update(GameTime gameTime) 
        {
            bool elapsed = base.Update(gameTime);

            if (elapsed)
            {
                if (_elapseTime > _bounds)
                {
                    if (_elapseTime * _modifier <= _bounds)
                    {
                        _elapseTime = _bounds;
                    }
                    else
                    {
                        _elapseTime *= _modifier;
                    }
                }
            }

            return elapsed;
        }

        public XElement Dehydrate()
        {
            XElement xVarTimer = new XElement("VariableTimer",
                new XAttribute("Bounds", _bounds),
                new XAttribute("Modifier", _modifier)
                );

            // Add super class as a child to this element.
            XElement xSuper = base.Dehydrate();
            xVarTimer.Add(xSuper);

            return xVarTimer;
        }
    }
}
