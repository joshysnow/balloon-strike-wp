using System;
using Microsoft.Xna.Framework;

namespace GameCore.Triggers
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

        public void Update(GameTime gameTime)
        {
            if (CanTrigger(gameTime))
                RaiseTriggered();
        }

        protected abstract bool CanTrigger(GameTime gameTime);

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
