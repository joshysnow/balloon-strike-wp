using System;
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
        /// <param name="elapseTime"></param>
        /// <param name="modifier">Percentage to apply to elapse time for every elapse.</param>
        /// <param name="bounds"></param>
        public VariableTimer(float elapseTime, float modifier, float bounds)
            : base(elapseTime)
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
    }
}
