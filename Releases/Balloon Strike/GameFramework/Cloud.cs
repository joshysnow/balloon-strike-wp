using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFramework
{
    public enum CloudState : byte
    {
        TransitionOn    = 0x01,
        OnScreen        = 0x02,
        TransitionOff   = 0x04
    }

    public enum CloudType : byte
    {
        Small   = 0x01,
        Medium  = 0x02,
        Large   = 0x04
    }

    public class Cloud : Character
    {
        public CloudState State
        {
            get;
            private set;
        }

        public CloudType Type
        {
            get;
            private set;
        }

        private short _screenWidth;

        public Cloud() : base()
        {
            State = CloudState.TransitionOff;
        }

        public void Initialize(CloudType type, Animation move, Vector2 positionUL, Vector2 velocity, short screenWidth)
        {
            Type = type;
            _moveAnimation = move;
            _positionUL = positionUL;
            _velocity = velocity;
            _screenWidth = screenWidth;

            int width = (int)(move.FrameWidth * move.Scale);
            int height = (int)(move.FrameHeight * move.Scale);
            _positionLR = new Vector2(positionUL.X + width, positionUL.Y + height);

            State = CloudState.TransitionOn;

            _animationPlayer.SetAnimation(move, positionUL);       
        }

        public override void Update(GameTime gameTime)
        {
            switch (State)
            {
                case CloudState.TransitionOn:
                    UpdateTransitionOn();
                    break;
                case CloudState.OnScreen:
                    UpdateOnScreen();
                    break;
                case CloudState.TransitionOff:
                default:
                    break;
            }

            _animationPlayer.UpdateAnimationPosition(_positionUL);
            base.Update(gameTime);
        }

        private void UpdateTransitionOn()
        {
            if (_positionLR.X <= _screenWidth)
            {
                State = CloudState.OnScreen;
            }

            UpdatePosition();
        }

        private void UpdateOnScreen()
        {
            if (_positionLR.X <= 0)
            {
                State = CloudState.TransitionOff;
            }

            UpdatePosition();
        }
    }
}
