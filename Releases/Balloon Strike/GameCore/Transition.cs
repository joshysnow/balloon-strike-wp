using System;
using Microsoft.Xna.Framework;

namespace GameCore
{
    public enum TransitionState : byte
    {
        Active = 0x01,
        TransitionOn = 0x02,
        TransitionOff = 0x04,
        Hidden = 0x08
    }

    public class Transition
    {
        public TransitionState State
        {
            get;
            set;
        }

        public TimeSpan TransitionOnTime
        {
            get;
            set;
        }

        public TimeSpan TransitionOffTime
        {
            get;
            set;
        }

        public TimeSpan ActiveTime
        {
            get;
            set;
        }

        public float TransitionAlpha
        {
            get { return 1f - _transitionPosition; }
        }

        public float TransitionPosition
        {
            get { return _transitionPosition; }
        }

        /// <summary>
        /// Will wait to transition off if true.
        /// </summary>
        public bool Invoked
        {
            get;
            set;
        }

        private float _transitionPosition;
        private float _activePosition;

        public Transition()
        {
            State = TransitionState.TransitionOn;
            _transitionPosition = 1;
            _activePosition = 0;
            TransitionOnTime = TimeSpan.Zero;
            TransitionOffTime = TimeSpan.Zero;
            ActiveTime = TimeSpan.Zero;
            Invoked = false;
        }

        public void Update(GameTime gameTime)
        {
            if (State == TransitionState.TransitionOn)
            {
                if (UpdateTransition(gameTime, TransitionOnTime, -1) == false)
                {
                    State = TransitionState.Active;
                }
            }

            if (State == TransitionState.Active && (Invoked == false))
            {
                if (!UpdateActiveTransition(gameTime))
                {
                    State = TransitionState.TransitionOff;
                }
            }

            if (State == TransitionState.TransitionOff)
            {
                if (UpdateTransition(gameTime, TransitionOffTime, 1) == false)
                {
                    State = TransitionState.Hidden;
                }
            }
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float delta = GetDelta(time, gameTime);

            _transitionPosition += delta * direction;

            if ((direction < 0 && _transitionPosition <= 0) || (direction > 0 && _transitionPosition >= 1))
            {
                _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }

            return true;
        }

        private bool UpdateActiveTransition(GameTime gameTime)
        {
            float delta = GetDelta(ActiveTime, gameTime);

            _activePosition += delta;

            if (_activePosition >= 1)
            {
                _activePosition = 0;
                return false;
            }

            return true;
        }

        private float GetDelta(TimeSpan time, GameTime gameTime)
        {
            float delta;

            if (time == TimeSpan.Zero)
            {
                delta = 1;
            }
            else
            {
                delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
            }

            return delta;
        }
    }
}
