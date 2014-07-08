using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace GameFramework
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
        Dying   = 0x0F,
        Frozen  = 0x10
    }

    public class Balloon : Character
    {
        public BalloonColor Color
        {
            get;
            set;
        }

        public BalloonState State
        {
            get { return _state; }
        }

        public float Health
        {
            get;
            private set;
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        //private Animation _freezeAnimation;
        private SoundEffect _popSound;
        private SimpleTimer _frozenTimer;
        private BalloonState _state;
        private bool _initialized;
        private bool _isAvailable;

        public Balloon() : base()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Initialize(Animation moveAnimation, Animation popAnimation, SoundEffect popSound, Vector2 position, Vector2 velocity, float health)
        {
            _popAnimation = popAnimation;
            _moveAnimation = moveAnimation;
            _popSound = popSound;
            _positionUL = position;
            _velocity = velocity;
            Health = health;

            int width = (int)(moveAnimation.FrameWidth * moveAnimation.Scale);
            int height = (int)(moveAnimation.FrameHeight * moveAnimation.Scale);
            _positionLR = new Vector2(position.X + width, position.Y + height);

            _state = BalloonState.Alive;

            _animationPlayer.SetAnimation(_moveAnimation, _positionUL);

            _initialized = true;
            _isAvailable = false;
        }

        public void Uninitialize()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (_initialized)
            {
                switch (_state)
                {
                    case BalloonState.Alive:
                        {
                            UpdateAlive();
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
                            UpdateDying();
                        }
                        break;
                    case BalloonState.Frozen:
                        {
                            UpdateFrozen(gameTime);
                        }
                        break;
                    case BalloonState.Dead:
                    default:
                        break;
                }

                base.Update(gameTime);   
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_initialized)
            {
                base.Draw(spriteBatch);                
            }
        }

        public void Pop()
        {
            if (!_initialized)
            {
                return;
            }

            _state = BalloonState.Popped;
            _popSound.Play();

            float centerX = (_positionUL.X + _positionLR.X) / 2;
            float centerY = (_positionUL.Y + _positionLR.Y) / 2;

            float width = (_popAnimation.FrameWidth * _popAnimation.Scale) / 2;
            float height = (_popAnimation.FrameHeight * _popAnimation.Scale) / 2;

            Vector2 explosionCoordinate = new Vector2((centerX - width), (centerY - height));

            _animationPlayer.SetAnimation(_popAnimation, _positionUL);
        }

        public void Freeze(int time)
        {
            if (!_initialized)
            {
                return;
            }

            _frozenTimer = new SimpleTimer();
            _frozenTimer.Initialize(time);

            //_animationPlayer.SetAnimation(_freezeAnimation, _positionUL);

            _state = BalloonState.Frozen;
        }

        public void Attack(float damage)
        {
            if (Health >= (1f / 8196f))
            {
                Health = MathHelper.Clamp((Health - damage), 0f, float.MaxValue);
            }
            else
            {
                Health = 0f;
            }

            if (Health == 0)
            {
                Pop();
            }
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
            }
        }

        private void UpdateFrozen(GameTime gameTime)
        {
            if (_frozenTimer == null)
            {
                return;
            }

            if (_frozenTimer.Update(gameTime))
            {
                _frozenTimer = null;
                //_animationPlayer.SetAnimation(_moveAnimation, _positionUL);
                _state = BalloonState.Alive;
            }
        }
    }
}
