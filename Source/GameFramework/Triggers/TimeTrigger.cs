using System;
using Microsoft.Xna.Framework;
using GameCore.Triggers;

namespace GameFramework.Triggers
{
    public class TimeTrigger : Trigger
    {
        private float _triggerTime;
        private float _timePassed;

        public TimeTrigger(TimeSpan triggerTime)
        {
            _triggerTime = (float)triggerTime.TotalMilliseconds;
            _timePassed = 0;
        }

        protected override bool CanTrigger(GameTime gameTime)
        {
            bool trigger = false;

            if (_timePassed >= _triggerTime)
            {
                trigger = true;
            }
            else
            {
                _timePassed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            return trigger;
        }
    }
}
