using System;
using Microsoft.Xna.Framework;

namespace GameCore.Triggers
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
