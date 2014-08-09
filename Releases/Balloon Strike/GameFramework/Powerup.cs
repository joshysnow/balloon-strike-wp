using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum PowerupType : byte
    {
        Freeze          = 0x01,
        Nuke            = 0x02,
        Shell           = 0x04,
        Rocket         = 0x08
    }

    public enum PowerupState : byte
    {
        Descending  = 0x01,
        Dead        = 0x02,
        Pickedup    = 0x03,
        Missed      = 0x04,
        PickingUp   = 0x08
    }

    public class Powerup : Character
    {
        private Animation _pickupAnimation;
        private SoundEffect _pickedUpSound;
        private PowerupState _state;
        private short _yLimit;
        private bool _initialized;
        private bool _isAvailable;

        public PowerupType Type
        {
            get; 
            private set;
        }

        public PowerupState State
        {
            get { return _state; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        public Powerup(PowerupType type) : base()
        {
            Type = type;
            _initialized = false;
            _isAvailable = true;
        }   

        public void Initialize(Animation moveAnimation, Animation pickupAnimation, SoundEffect pickedUp, Vector2 position, Vector2 velocity, short yLimit)
        {
            _staticAnimation = moveAnimation;
            _pickupAnimation = pickupAnimation;
            _pickedUpSound = pickedUp;
            _positionUL = position;
            _velocity = velocity;
            _yLimit = yLimit;

            int width = (int)(moveAnimation.FrameWidth * moveAnimation.Scale);
            int height = (int)(moveAnimation.FrameHeight * moveAnimation.Scale);
            _positionLR = new Vector2(position.X + width, position.Y + height);
            _rectangle = new GameCore.Physics.Shapes.Rectangle(_positionUL, _positionLR);

            _state = PowerupState.Descending;

            _animationPlayer.SetAnimation(moveAnimation);
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

        public void Pickup()
        {
            if (!_initialized || (_state != PowerupState.Descending))
            {
                return;
            }

            _state = PowerupState.PickingUp;
            _pickedUpSound.Play();

            _animationPlayer.SetAnimation(_pickupAnimation);
            _animationPlayer.SetPosition(_positionUL);
        }

        private void UpdateDescending()
        {
            if (_positionUL.Y >= _yLimit)
            {
                _state = PowerupState.Missed;
            }

            UpdatePosition();

            _animationPlayer.SetPosition(_positionUL);
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
