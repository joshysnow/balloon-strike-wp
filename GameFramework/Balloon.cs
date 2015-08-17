using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameCore.Timers;

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
        Frozen  = 0x10,
        Hit     = 0x20
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

        private Animation _popAnimation;
        private Animation _hitAnimation;
        private SoundEffect _popSound;
        private SimpleTimer _frozenTimer;
        private BalloonState _state;
        private BalloonState _previousState;
        private bool _initialized;
        private bool _isAvailable;

        public Balloon() : base()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Initialize(BalloonModel model)
        {
            // TODO: Perhaps initialization of pooled nodes could be inherited? Or an object to hold the state of initialized/available.
        }

        public void Initialize(Animation moveAnimation, Animation hitAnimation, Animation popAnimation, SoundEffect popSound, Vector2 position, Vector2 velocity, float health)
        {
            _popAnimation = popAnimation;
            _hitAnimation = hitAnimation;
            _moveAnimation = moveAnimation;
            _popSound = popSound;
            _positionUL = position;
            _velocity = velocity;
            Health = health;

            int width = (int)(moveAnimation.FrameWidth * moveAnimation.Scale);
            int height = (int)(moveAnimation.FrameHeight * moveAnimation.Scale);
            _positionLR = new Vector2(position.X + width, position.Y + height);
            _rectangle = new GameCore.Physics.Shapes.Rectangle(_positionUL, _positionLR);

            _state = BalloonState.Alive;
            _previousState = BalloonState.Alive;

            _animationPlayer.SetAnimation(_moveAnimation);
            _animationPlayer.SetPosition(_positionUL);

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
                    case BalloonState.Hit:
                        {
                            UpdateHit(gameTime);
                        }
                        break;
                    case BalloonState.Popped:
                        {
                            ChangeState(BalloonState.Dying);
                        }
                        break;
                    case BalloonState.Escaped:
                        {
                            ChangeState(BalloonState.Dead);
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

        public void Freeze(int time)
        {
            if (!_initialized)
            {
                return;
            }

            _frozenTimer = new SimpleTimer(time);
            //_animationPlayer.SetAnimation(_freezeAnimation, _positionUL);
            ChangeState(BalloonState.Frozen);
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
            else
            {
                ChangeState(BalloonState.Hit);
                _animationPlayer.SetAnimation(_hitAnimation);
            }
        }

        private void Pop()
        {
            if (!_initialized)
            {
                return;
            }

            ChangeState(BalloonState.Popped);
            _popSound.Play();

            float centerX = (_positionUL.X + _positionLR.X) / 2;
            float centerY = (_positionUL.Y + _positionLR.Y) / 2;

            float width = (_popAnimation.FrameWidth * _popAnimation.Scale) / 2;
            float height = (_popAnimation.FrameHeight * _popAnimation.Scale) / 2;

            Vector2 explosionCoordinate = new Vector2((centerX - width), (centerY - height));

            _animationPlayer.SetAnimation(_popAnimation);
            _animationPlayer.SetPosition(_positionUL);
        }

        private void UpdateAlive()
        {
            if (_positionLR.Y <= 0)
            {
                ChangeState(BalloonState.Escaped);
            }

            UpdatePosition();

            _animationPlayer.SetPosition(_positionUL);
        }

        private void UpdateHit(GameTime gameTime)
        {
            if (_animationPlayer.Finished)
            {
                ChangeState(_previousState);
                _animationPlayer.SetAnimation(_moveAnimation);
            }

#warning What if frozen timer finishes before hit animation has ended? Make it sync with animation duration
            switch (_previousState)
            {
                case BalloonState.Hit:
                case BalloonState.Alive:
                    UpdateAlive();
                    break;
                case BalloonState.Frozen:
                    UpdateFrozen(gameTime);
                    break;
                case BalloonState.Dead:
                case BalloonState.Popped:
                case BalloonState.Escaped:
                case BalloonState.Dying:
                    break;
                default:
                    break;
            }
        }

        private void UpdateDying()
        {
            if (_animationPlayer.Finished)
            {
                ChangeState(BalloonState.Dead);
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

                if (_state == BalloonState.Hit)
                {
                    _previousState = BalloonState.Alive;
                }
                else
                {
                    ChangeState(BalloonState.Alive);
                }                
            }
        }

        private void ChangeState(BalloonState state)
        {
            _previousState = _state;
            _state = state;
        }
    }
}
