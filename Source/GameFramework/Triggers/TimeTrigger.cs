using System;
using Microsoft.Xna.Framework;

namespace GameFramework.Triggers
{
    public class TimeTrigger : Trigger
    {
        private float _timePassed;

        public TimeTrigger(TimeSpan triggerTime)
        {
            _triggerPoint = (float)triggerTime.TotalMilliseconds;
            _timePassed = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (_timePassed >= _triggerPoint)
            {
                RaiseTriggered();
            }
            else
            {
                _timePassed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }
}
