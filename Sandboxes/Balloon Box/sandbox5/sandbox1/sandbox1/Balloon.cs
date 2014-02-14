using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox5
{
    public enum BalloonColor : byte
    {
        Red     = 0x01,
        Green   = 0x02,
        Blue    = 0x04
    }

    public enum BalloonState : byte
    {
        Alive   = 0x01,
        Dead    = 0x02,
        Popped  = 0x04,
        Escaped = 0x08,
        Dying   = 0x0F
    }

    public class Balloon
    {
        private AnimationPlayer _animationPlayer;
        private Animation _popAnimation;
        private Animation _moveAnimation;
        private SoundEffect _popSound;
        private Vector2 _positionUL;
        private Vector2 _positionLR;
        private Vector2 _velocity;
        private BalloonState _state;
        private bool _initialized;
        private bool _isAvailable;

        public BalloonColor Color
        {
            get; set;
        }

        public BalloonState State
        {
            get { return _state; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        public Balloon()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Initialize(Animation moveAnimation, Animation popAnimation, SoundEffect popSound, Vector2 position, Vector2 velocity)
        {
            _popAnimation = popAnimation;
            _moveAnimation = moveAnimation;
            _popSound = popSound;
            _positionUL = position;
            _velocity = velocity;

            float width = (moveAnimation.FrameWidth * moveAnimation.Scale);
            float height = (moveAnimation.FrameWidth * moveAnimation.Scale);
            _positionLR = new Vector2(position.X + width, position.Y + height);

            _state = BalloonState.Alive;

            _animationPlayer = new AnimationPlayer();
            _animationPlayer.SetAnimation(_moveAnimation, _positionUL);

            _initialized = true;
            _isAvailable = false;
        }

        public void Uninitialize()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!_initialized)
            {
                return;
            }
            
            switch (_state)
            {
                case BalloonState.Alive:
                    {
                        this.UpdateAlive();
                    }
                    break;
                case BalloonState.Popped:
                    {
                        _state = BalloonState.Dying;
                    }
                    break;
                case BalloonState.Escaped:
                    {
                        _state = BalloonState.Dead;
                    }
                    break;
                case BalloonState.Dying:
                    {
                        this.UpdateDying();
                    }
                    break;
                case BalloonState.Dead:
                default:
                    break;
            }

            _animationPlayer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_initialized)
            {
                return;
            }

            _animationPlayer.Draw(spriteBatch);
        }

        public bool Intersects(Vector2 position)
        {
            if (!_initialized)
            {
                return false;
            }

            return Collisions.Intersects(_positionUL, _positionLR, position);
        }

        public void Pop()
        {
            if (!_initialized || (_state != BalloonState.Alive))
            {
                return;
            }

            _state = BalloonState.Popped;
            _popSound.Play();

            float centerX = (_positionUL.X + _positionLR.X) / 2;
            float centerY = (_positionUL.Y + _positionLR.Y) / 2;

            float width = (_popAnimation.AnimationTexture.Width * _popAnimation.Scale) / 2;
            float height = (_popAnimation.AnimationTexture.Height * _popAnimation.Scale) / 2;

            Vector2 explosionCoordinate = new Vector2((centerX - width), (centerY - height));

            _animationPlayer.SetAnimation(_popAnimation, explosionCoordinate);
        }

        private void UpdateAlive()
        {
            if (_positionLR.Y <= 0)
            {
                _state = BalloonState.Escaped;
            }

            _positionUL += _velocity;
            _positionLR += _velocity;

            _animationPlayer.UpdateAnimationPosition(_positionUL);
        }

        private void UpdateDying()
        {
            if (_animationPlayer.Finished)
            {
                _state = BalloonState.Dead;
                return;
            }
        }
    }
}