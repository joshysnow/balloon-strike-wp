using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace sandbox4
{
    public enum BalloonColour : byte
    {
        Red     = 0x01,
        Green   = 0x02,
        Blue    = 0x04
    }

    public enum BalloonState : byte
    {
        Alive   = 0x01,
        Dead    = 0x02,
        Popped  = 0x04
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
        private Vector2 _origin;
        private BalloonState _state;
        private bool _initialized;
        private bool _isAvailable;
        private float _scale;
        private int height;
        private int width;

        public BalloonColour Colour
        {
            get; set;
        }

        public BalloonState State
        {
            get { return _state; }
        }

        public Vector2 Position
        {
            get { return _positionUL; }
        }

        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
        }

        public Balloon()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Initialize(Animation moveAnimation, Animation popAnimation, SoundEffect popSound, Vector2 position, Vector2 velocity, float scale)
        {
            _popAnimation = popAnimation;
            _moveAnimation = moveAnimation;
            _popSound = popSound;
            _positionUL = position;
            _velocity = velocity;
            _scale = scale;

            width = (int)(moveAnimation.AnimationTexture.Width * scale);
            height = (int)(moveAnimation.AnimationTexture.Height * scale);

            _positionLR = new Vector2(position.X + width, position.Y + height);
            _origin = new Vector2(width, height);

            _state = BalloonState.Alive;

            _animationPlayer = new AnimationPlayer();
            _animationPlayer.SetAnimation(_moveAnimation, _positionUL);

            _initialized = true;
        }

        public void Uninitialize()
        {
            _isAvailable = true;
            _initialized = false;
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
                        this.UpdateAlive(gameTime);
                    }
                    break;
                case BalloonState.Dead:
                    {
                        this.UpdateDead();
                    }
                    break;
                case BalloonState.Popped:
                    {
                        this.UpdatePopped(gameTime);
                    }
                    break;
                default:
                    break;
            }
        }

        public bool Intersects(Vector2 position)
        {
            if (position.X >= _positionUL.X && position.X <= _positionLR.X &&
                position.Y >= _positionUL.Y && position.Y <= _positionLR.Y)
            {
                return true;
            }

            return false;
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

            float width = (_popAnimation.AnimationTexture.Width * 0.25f / 2);
            float height = (_popAnimation.AnimationTexture.Height * 0.25f) / 2;

            Vector2 explosionCoordinate = new Vector2((centerX - width), (centerY - height));

            _animationPlayer.SetAnimation(_popAnimation, explosionCoordinate);
        }

        private void UpdateAlive(GameTime gameTime)
        {
            if (_positionLR.Y <= 0)
            {
                _state = BalloonState.Dead;
            }

            _positionUL += _velocity;
            _positionLR += _velocity;

            _animationPlayer.UpdateAnimationPosition(_positionUL);
            _animationPlayer.Update(gameTime);
        }

        private void UpdateDead()
        {

        }

        private void UpdatePopped(GameTime gameTime)
        {
            if (_animationPlayer.Finished)
            {
                _state = BalloonState.Dead;
                return;
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
    }
}
