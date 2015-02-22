using System;
using Microsoft.Xna.Framework;

namespace GameFramework.Triggers
{
    public delegate void TriggerHandler(Trigger trigger);

    public abstract class Trigger
    {
        public bool HasTriggered
        {
            get;
            private set;
        }

        public event TriggerHandler Triggered;

        protected float _triggerPoint;

        protected void RaiseTriggered()
        {
            if (HasTriggered)
            {
                return;
            }

            HasTriggered = true;

            if (Triggered != null)
            {
                Triggered(this);
            }
        }
    }
}
