using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameInterfaceFramework
{
    public class View
    {
        public TransitionState State
        {
            get
            {
                return _transition.State;
            }
        }

        public ViewManager ViewManager
        {
            get;
            internal set;
        }

        public GestureType EnabledGestures
        {
            get;
            protected set;
        }

        public float TransitionAlpha
        {
            get
            {
                return _transition.TransitionAlpha;
            }
        }

        public Transition Transition
        {
            get
            {
                return _transition;
            }
        }

        public bool IsPopup
        {
            get;
            set;
        }

        public bool IsExiting
        {
            get
            {
                return _isExiting;
            }
        }

        private Transition _transition = new Transition() { Invoked = true };
        private bool _isExiting = false;

        public void Exit()
        {
            //if (_transitionOffTime == TimeSpan.Zero)
            //{
            //    _state = ViewState.Hidden;
            //}

            _transition.State = TransitionState.TransitionOff;

            _isExiting = true;
        }

        public virtual void Activate(bool instancePreserved) { }

        public virtual void Deactivate() { }

        public virtual void HandlePlayerInput(ControlsState controls) { }

        public virtual void Update(GameTime gameTime, bool covered)
        {
            if (IsExiting)
            {
                if (_transition.State == TransitionState.Hidden)
                {
                    ViewManager.RemoveView(this);
                }

                //_state = ViewState.TransitionOff;

                //if (!UpdateTransition(gameTime, _transitionOffTime, 1))
                //{
                //    ViewManager.RemoveView(this);
                //}
            }
            
            if (covered)
            {
                if ((_transition.State == TransitionState.Active) || (_transition.State == TransitionState.TransitionOn))
                {
                    _transition.State = TransitionState.TransitionOff;
                }

                //if (UpdateTransition(gameTime, _transitionOffTime, 1))
                //{
                //    _state = ViewState.TransitionOff;
                //}
                //else
                //{
                //    _state = ViewState.Hidden;
                //}
            }
            //else
            //{
            //    if (UpdateTransition(gameTime, _transitionOnTime, -1))
            //    {
            //        _state = ViewState.TransitionOn;
            //    }
            //    else
            //    {
            //        _state = ViewState.Active;
            //    }
            //}

            _transition.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime) { }

        //private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        //{
        //    float delta;

        //    if (time == TimeSpan.Zero)
        //    {
        //        delta = 1;
        //    }
        //    else
        //    {
        //        delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
        //    }

        //    _transitionPosition += delta * direction;

        //    if ((direction < 0 && _transitionPosition <= 0) || (direction > 0 && _transitionPosition >= 1))
        //    {
        //        _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
        //        return false;
        //    }

        //    return true;
        //}
    }
}
