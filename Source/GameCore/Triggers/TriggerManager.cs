using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameCore.Triggers
{
    public class TriggerManager
    {
        private List<Trigger> _triggers;

        public TriggerManager()
        {
            _triggers = new List<Trigger>();
        }

        public void AddTrigger(Trigger trigger)
        {
            if (!_triggers.Contains(trigger))
            {
                _triggers.Add(trigger);
            }
        }

        public void Remove(Trigger trigger)
        {
            if (_triggers.Contains(trigger))
            {
                _triggers.Remove(trigger);
            }
        }

        public void Clear()
        {
            _triggers.Clear();
        }

        public void Update(GameTime gameTime)
        {
            byte index = 0;
            Trigger trigger;
            while (index < _triggers.Count)
            {
                trigger = _triggers[index];
                trigger.Update(gameTime);

                if (trigger.HasTriggered)
                {
                    _triggers.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }
    }
}
