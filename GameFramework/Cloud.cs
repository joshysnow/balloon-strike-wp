using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum CloudState : byte
    {
        OnScreen    = 0x01,
        OffScreen   = 0x02
    }

    public enum CloudType : byte
    {
        Small   = 0x01,
        Medium  = 0x02
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
            get { return _model.Type; }
        }

        public Vector2 OriginalPosition
        {
            get;
            set;
        }

        private CloudModel _model;
        private int _screenWidth;

        public Cloud() : base()
        {
            State = CloudState.OffScreen;
        }

        public void Initialize(CloudModel model, Vector2 positionUL, int screenWidth)
        {
            _model = model;
            _positionUL = positionUL;
            _velocity = model.Velocity;
            _screenWidth = screenWidth;

            Animation move = model.MoveAnimation;
            int width = (int)(move.FrameWidth * move.Scale);
            int height = (int)(move.FrameHeight * move.Scale);
            _positionLR = new Vector2(positionUL.X + width, positionUL.Y + height);
            _rectangle = new GameCore.Physics.Shapes.Rectangle(_positionUL, _positionLR);

            State = CloudState.OnScreen;

            _animationPlayer.SetAnimation(move);
            _animationPlayer.SetPosition(positionUL);
        }

        public override void Update(GameTime gameTime)
        {
            if (State == CloudState.OnScreen)
            {
                UpdateOnScreen();
            }

            base.Update(gameTime);
        }

        private void UpdateOnScreen()
        {
            if (_positionLR.X <= 0)
            {
                State = CloudState.OffScreen;
            }

            UpdatePosition();
            _animationPlayer.SetPosition(_positionUL);
        }
    }
}
