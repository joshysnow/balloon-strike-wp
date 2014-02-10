using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace sandbox3
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
        private Texture2D _balloonTexture;
        private Texture2D _popTexture;
        private SoundEffect _popSound;
        private Vector2 _positionUL;
        private Vector2 _positionLR;
        private Vector2 _explosionCoordinate;
        private Vector2 _velocity;
        private Vector2 _origin;
        private BalloonState _state;
        private bool _initialized;
        private bool _isAvailable;
        private float _scale;
        private float _animationDuration;
        private float _animationElapsed;
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
            _animationDuration = 125f;
            _animationElapsed = 0f;
        }

        public void Initialize(ref Texture2D ballTexture, ref Texture2D popTexture, ref SoundEffect pop, Vector2 position, Vector2 velocity, float scale)
        {
            _balloonTexture = ballTexture;
            _popTexture = popTexture;
            _popSound = pop;
            _positionUL = position;
            _velocity = velocity;
            _scale = scale;

            width = (int)(_balloonTexture.Width * scale);
            height = (int)(_balloonTexture.Height * scale);

            _positionLR = new Vector2(position.X + width, position.Y + height);
            _origin = new Vector2(width, height);

            _state = BalloonState.Alive;

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
                        this.UpdateAlive();
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

            float width = (_popTexture.Width * 0.25f / 2);
            float height = (_popTexture.Height * 0.25f) / 2;

            _explosionCoordinate = new Vector2((centerX - width), (centerY - height));
        }

        private void UpdateAlive()
        {
            if (_positionLR.Y <= 0)
            {
                _state = BalloonState.Dead;
            }

            _positionUL += _velocity;
            _positionLR += _velocity;
        }

        private void UpdateDead()
        {

        }

        private void UpdatePopped(GameTime gameTime)
        {
            if (_animationElapsed >= _animationDuration)
            {
                _state = BalloonState.Dead;
                _animationElapsed = 0f;
            }
            else
            {
                _animationElapsed += gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public void Draw(ref SpriteBatch spriteBatch)
        {
            if (!_initialized)
            {
                return;
            }

            switch (_state)
            {
                case BalloonState.Alive:
                    {
                        this.DrawBalloon(ref spriteBatch);
                    }
                    break;

                case BalloonState.Popped:
                    {
                        this.DrawExplosion(ref spriteBatch);
                    }
                    break;
                case BalloonState.Dead:
                default:
                    break;
            }
        }

        private void DrawBalloon(ref SpriteBatch spriteBatch)
        {
            spriteBatch.Draw( _balloonTexture, _positionUL, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }

        private void DrawExplosion(ref SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_popTexture, _explosionCoordinate, null, Color.White, 0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
        }
    }
}
