using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    public class Pulse
    {
        private enum PulseState : byte
        {
            Wait        = 0x01,
            Transition  = 0x02
        }

        public float Position
        {
            get;
            private set;
        }

        private PulseState _state;
        private bool _increase;
        private float _time;
        private float _duration;
        private float _frequency;

        public Pulse()
        {
            Position = 0;
            _frequency = 0;
            _increase = true;
            _state = PulseState.Wait;
        }

        public Pulse(TimeSpan duration) : this()
        {
            _duration = (float)duration.TotalMilliseconds / 2f;
        }

        public Pulse(TimeSpan duration, TimeSpan frequency) : this(duration)
        {
            _frequency = (float)frequency.TotalMilliseconds;
        }

        public void Update(GameTime gameTime)
        {
            switch (_state)
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
                _state = PulseState.Wait;
                _time = 0;
                _increase = true;
            }
        }

        private void UpdateWaitState(GameTime gameTime)
        {
            _time -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            _time = MathHelper.Clamp(_time, 0, _frequency);

            if (_time == 0 && _duration > 0f)
            {
                _state = PulseState.Transition;
            }
        }

        private void UpdateTransitionState(GameTime gameTime)
        {
            if (_increase)
                _time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                _time -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            _time = MathHelper.Clamp(_time, 0, _duration);

            Position = _time / _duration;

            if (_time == _duration)
            {
                _increase = false;
            }

            if (_time == 0f)
            {
                _increase = true;

                if (_frequency > 0)
                {
                    _state = PulseState.Wait;
                    _time = _frequency;
                }
            }
        }
    }
}
