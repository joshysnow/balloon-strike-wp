using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameInterfaceFramework
{
    public delegate void ViewExitHandler(View view);

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

        public GestureType ViewGestures
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

        public event ViewExitHandler ViewExiting;

        private Transition _transition = new Transition() { Invoked = true };
        private bool _isExiting = false;

        public void Exit()
        {
            _transition.State = TransitionState.TransitionOff;
            _isExiting = true;

            if (ViewExiting != null)
                ViewExiting(this);
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
            }
            
            if (covered)
            {
                if ((_transition.State == TransitionState.Active) || (_transition.State == TransitionState.TransitionOn))
                {
                    _transition.State = TransitionState.TransitionOff;
                }
            }

            _transition.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime) { }
    }
}
