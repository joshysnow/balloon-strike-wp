using System;
using Microsoft.Xna.Framework;

namespace GameInterfaceFramework
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

        public TimeSpan TransitionOn
        {
            get;
            set;
        }

        public TimeSpan TransitionOff
        {
            get;
            set;
        }

        public float TransitionAlpha
        {
            get { return 1f - _transitionPosition; }
        }

        private float _transitionPosition;

        public Transition()
        {
            State = TransitionState.TransitionOn;
            _transitionPosition = 1f;
            TransitionOn = TimeSpan.Zero;
            TransitionOff = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (State == TransitionState.TransitionOn)
            {
                if (UpdateTransition(gameTime, TransitionOn, -1) == false)
                {
                    State = TransitionState.Active;
                }
            }

            if (State == TransitionState.TransitionOff)
            {
                if (UpdateTransition(gameTime, TransitionOn, -1) == false)
                {
                    State = TransitionState.Hidden;
                }
            }
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
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

            _transitionPosition += delta * direction;

            if ((direction < 0 && _transitionPosition <= 0) || (direction > 0 && _transitionPosition >= 1))
            {
                _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }

            return true;
        }
    }
}
