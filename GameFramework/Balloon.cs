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

    public class Balloon : Character, ISpawnable
    {
        public string SpawnType
        {
            get 
            { 
                return "Balloon" + Color; 
            }
        }

        public BalloonColor Color
        {
            get;
            set;
        }

        public BalloonState State
        {
            get { return _currentState; }
        }

        public Vector2 Size
        {
            get { return _model.Size; }
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

        private SimpleTimer _frozenTimer;
        private BalloonState _currentState;
        private BalloonState _previousState;
        private BalloonModel _model;
        private bool _initialized;
        private bool _isAvailable;

        public Balloon() : base()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Initialize(BalloonModel model, ref Vector2 position, float health)
        {
            _model = model;
            Health = health;

            // Set default state.
            _currentState = BalloonState.Alive;
            _previousState = BalloonState.Alive;

            // Set positions.
            _positionUL = position;
            _positionLR = new Vector2(
                (position.X + model.Size.X),    // x
                (position.Y + model.Size.Y));   // y

            // Instantiate collision shape.
            _rectangle = new GameCore.Physics.Shapes.Rectangle(_positionUL, _positionLR);

            // TODO: Change inheritance hierarchy to be more component based i.e., passed in a character.
            _velocity = _model.Velocity;

            // Initialize animation player.
            _animationPlayer.SetPosition(_positionUL);
            _animationPlayer.SetAnimation(_model.MoveAnimation);

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
                switch (_currentState)
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
                _animationPlayer.SetAnimation(_model.HitAnimation);
            }
        }

        private void Pop()
        {
            if (!_initialized)
            {
                return;
            }

            ChangeState(BalloonState.Popped);
            _model.PopSound.Play();

            float centerX = (_positionUL.X + _positionLR.X) / 2;
            float centerY = (_positionUL.Y + _positionLR.Y) / 2;

            float halfWidth = (_model.PopAnimation.FrameWidth * _model.PopAnimation.Scale) / 2;
            float halfHeight = (_model.PopAnimation.FrameHeight * _model.PopAnimation.Scale) / 2;

            // Essentially the coordinates should be centre - half the new textures width
            Vector2 explosionCoordinate = new Vector2((centerX - halfWidth), (centerY - halfHeight));

            _animationPlayer.SetAnimation(_model.PopAnimation);
            _animationPlayer.SetPosition(explosionCoordinate);
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
                _animationPlayer.SetAnimation(_model.MoveAnimation);
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

                if (_currentState == BalloonState.Hit)
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
            _previousState = _currentState;
            _currentState = state;
        }
    }
}
