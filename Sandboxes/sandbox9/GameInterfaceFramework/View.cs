using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameInterfaceFramework
{
    public enum ViewState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public class View
    {
        public bool IsPopup
        {
            get 
            { 
                return _isPopup; 
            }
        }

        public ViewState State
        {
            get
            {
                return _state;
            }
        }

        public bool IsExiting
        {
            get
            {
                return _isExiting;
            }
        }

        public ViewManager ViewManager
        {
            get;
            internal set;
        }

        public GestureType EnabledGestures
        {
            get
            {
                return _viewGestures;
            }
        }

        protected float TransitionAlpha
        {
            get
            {
                return 1f - _transitionPosition;
            }
        }

        protected GestureType _viewGestures = GestureType.None;
        protected TimeSpan _transitionOnTime = TimeSpan.Zero;
        protected TimeSpan _transitionOffTime = TimeSpan.Zero;
        protected bool _isPopup = false;
        private ViewState _state = ViewState.TransitionOn;
        private float _transitionPosition = 1f;
        private bool _isExiting = false;

        public void Exit()
        {
            if (_transitionOffTime == TimeSpan.Zero)
            {
                _state = ViewState.TransitionOff;
            }

            _isExiting = true;
        }

        public virtual void Activate(bool instancePreserved) { }

        public virtual void Deactivate() { }

        public virtual void HandlePlayerInput() { }

        public virtual void Update(GameTime gameTime, bool covered)
        {
            if (IsExiting)
            {
                _state = ViewState.TransitionOff;

                if (!UpdateTransition(gameTime, _transitionOffTime, 1))
                {

                }
            }
            else if (covered)
            {
                if (UpdateTransition(gameTime, _transitionOffTime, 1))
                {
                    _state = ViewState.TransitionOff;
                }
                else
                {
                    _state = ViewState.Hidden;
                }
            }
            else
            {
                if (UpdateTransition(gameTime, _transitionOnTime, -1))
                {
                    _state = ViewState.TransitionOn;
                }
                else
                {
                    _state = ViewState.Active;
                }
            }
        }

        public virtual void Draw(GameTime gameTime) { }

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
