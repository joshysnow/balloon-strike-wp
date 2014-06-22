using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox7
{
    public enum PowerupType : byte
    {
        Freeze = 0x01
    }

    public enum PowerupState : byte
    {
        Descending  = 0x01,
        Dead        = 0x02,
        Pickedup    = 0x03,
        Missed      = 0x04,
        PickingUp   = 0x08
    }

    public class Powerup
    {
        private AnimationPlayer _animationPlayer;
        private Animation _moveAnimation;
        private Animation _pickupAnimation;
        private SoundEffect _pickedUpSound;
        private Vector2 _positionUL;
        private Vector2 _positionLR;
        private Vector2 _velocity;
        private PowerupState _state;
        private short _yLimit;
        private bool _initialized;
        private bool _isAvailable;

        public PowerupType Type
        {
            get; set;
        }

        public PowerupState State
        {
            get { return _state; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        public Powerup()
        {
            _initialized = false;
            _isAvailable = true;
        }   

        public void Initialize(Animation moveAnimation, Animation pickupAnimation, SoundEffect pickedUp, Vector2 position, Vector2 velocity, short yLimit)
        {
            _moveAnimation = moveAnimation;
            _pickupAnimation = pickupAnimation;
            _pickedUpSound = pickedUp;
            _positionUL = position;
            _velocity = velocity;
            _yLimit = yLimit;

            int width = (int)(moveAnimation.FrameWidth * moveAnimation.Scale);
            int height = (int)(moveAnimation.FrameHeight * moveAnimation.Scale);
            _positionLR = new Vector2(position.X + width, position.Y + height);

            _state = PowerupState.Descending;

            _animationPlayer = new AnimationPlayer();
            _animationPlayer.SetAnimation(moveAnimation, _positionUL);

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
                case PowerupState.Descending:
                    {
                        this.UpdateDescending();
                    }
                    break;
                case PowerupState.Pickedup:
                case PowerupState.Missed:
                    {
                        _state = PowerupState.Dead;
                    }
                    break;
                case PowerupState.PickingUp:
                    {
                        this.UpdatePickingUp();
                    }
                    break;
                case PowerupState.Dead:
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

        public void Pickup()
        {
            if (!_initialized || (_state != PowerupState.Descending))
            {
                return;
            }

            _state = PowerupState.PickingUp;
            _pickedUpSound.Play();



            _animationPlayer.SetAnimation(_pickupAnimation, _positionUL);
        }

        private void UpdateDescending()
        {
            if (_positionUL.Y >= _yLimit)
            {
                _state = PowerupState.Missed;
            }

            _positionUL += _velocity;
            _positionLR += _velocity;

            _animationPlayer.UpdateAnimationPosition(_positionUL);
        }

        private void UpdatePickingUp()
        {
            if (_animationPlayer.Finished)
            {
                _state = PowerupState.Pickedup;
            }
        }
    }
}
