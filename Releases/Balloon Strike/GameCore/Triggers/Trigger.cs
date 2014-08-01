using System;
using Microsoft.Xna.Framework;

namespace GameCore.Triggers
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
}
