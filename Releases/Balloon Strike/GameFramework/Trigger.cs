using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    public delegate void TriggerHandler(Trigger trigger);

    public abstract class Trigger
    {
        public event TriggerHandler Triggered;
        protected float _triggerPoint;
        private bool _triggered = false;

        public bool HasTriggered
        {
            get { return _triggered; }
        }

        protected void RaiseTriggered()
        {
            if(_triggered)
            {
                return;
            }

            _triggered = true;

            if (Triggered != null)
            {
                Triggered(this);
            }
        }
    }

    public class ScoreTrigger : Trigger
    {
        public ScoreTrigger(int scoreToTriggerAt)
        {
            _triggerPoint = scoreToTriggerAt;
        }

        public void Update(int currentScore)
        {
            if (currentScore >= _triggerPoint)
            {
                RaiseTriggered();
            }
        }
    }

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
