using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox2
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
        Dead    = 0x02
    }

    public class Balloon
    {
        private Texture2D _texture;
        private Vector2 _positionUL;
        private Vector2 _positionLR;
        private Vector2 _velocity;
        private Vector2 _origin;
        private BalloonState _state;
        private bool _initialized;
        private bool _isAvailable;
        private float _scale;
        private float height;
        private float width;

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
        }

        public void Initialize(ref Texture2D texture, Vector2 position, Vector2 velocity, float scale)
        {
            _texture = texture;
            _positionUL = position;
            _velocity = velocity;
            _scale = scale;

            width = _texture.Width * scale;
            height = _texture.Height * scale;

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
                default:
                    break;
            }
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

        public void Draw(ref SpriteBatch spriteBatch)
        {
            if (!_initialized)
            {
                return;
            }

            spriteBatch.Draw(_texture, _positionUL, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }
    }
}
