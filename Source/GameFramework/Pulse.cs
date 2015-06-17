using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    public enum PulseState : byte
    {
        Wait = 0x01,
        Transition = 0x02
    }

    public class Pulse
    {
        public PulseState State
        {
            get;
            private set;
        }

        public float Position
        {
            get;
            private set;
        }

        public float Time
        {
            get;
            private set;
        }

        public bool Increasing
        {
            get;
            private set;
        }

        private float _duration;
        private float _frequency;

        public Pulse()
        {
            Position = 0;
            _frequency = 0;
            Increasing = true;
            State = PulseState.Wait;
        }

        public Pulse(TimeSpan duration) : this()
        {
            _duration = (float)duration.TotalMilliseconds / 2f;
        }

        public Pulse(TimeSpan duration, TimeSpan frequency) : this(duration)
        {
            _frequency = (float)frequency.TotalMilliseconds;
        }

        public void Activate(PulseState state, float position, float time, bool increasing)
        {
            State       = state;
            Position    = position;
            Time        = time;
            Increasing  = increasing;
        }

        public void Update(GameTime gameTime)
        {
            switch (State)
            {
                case PulseState.Wait:
                    UpdateWaitState(gameTime);
                    break;
                case PulseState.Transition:
                    UpdateTransitionState(gameTime);
                    break;
                default:
                    break;
            }
        }

        public void ChangeRhythm(TimeSpan duration, TimeSpan frequency, bool reset = false)
        {
            _duration = (float)duration.TotalMilliseconds;
            _frequency = (float)frequency.TotalMilliseconds;

            if (reset || _duration == 0f)
            {
                Position = 0;
                State = PulseState.Wait;
                Time = 0;
                Increasing = true;
            }
        }

        private void UpdateWaitState(GameTime gameTime)
        {
            Time -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            Time = MathHelper.Clamp(Time, 0, _frequency);

            if (Time == 0 && _duration > 0f)
            {
                State = PulseState.Transition;
            }
        }

        private void UpdateTransitionState(GameTime gameTime)
        {
            if (Increasing)
                Time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                Time -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            Time = MathHelper.Clamp(Time, 0, _duration);

            Position = Time / _duration;

            if (Time == _duration)
            {
                Increasing = false;
            }

            if (Time == 0)
            {
                Increasing = true;

                if (_frequency > 0)
                {
                    State = PulseState.Wait;
                    Time = _frequency;
                }
            }
        }
    }
}
