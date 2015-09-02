using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum PowerupType : byte
    {
        Freeze  = 0x01,
        Shell   = 0x04,
        Rocket  = 0x08
    }

    public enum PowerupState : byte
    {
        Falling     = 0x01,
        Dead        = 0x02,
        Pickedup    = 0x03,
        Missed      = 0x04,
        PickingUp   = 0x08
    }

    public class Powerup : Character, ISpawnable
    {
        public string SpawnType
        {
            get { return "Powerup_" + Type; }
        }

        public PowerupType Type
        {
            get;
            set;
        }

        public PowerupState State
        {
            get { return _state; }
        }

        public Vector2 Size
        {
            get { return _model.Size; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        private PowerupModel _model;
        private PowerupState _state;
        private int _maxY;
        private bool _initialized;
        private bool _isAvailable;

        public Powerup() : base()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public void Initialize(PowerupModel model, ref Vector2 position, int maxY)
        {
            _model = model;
            _maxY = maxY;

            _positionUL = position;
            _positionLR = new Vector2(position.X + model.Size.X, position.Y + model.Size.Y);
            _rectangle = new GameCore.Physics.Shapes.Rectangle(_positionUL, _positionLR);

            _velocity = model.Velocity;

            _state = PowerupState.Falling;

            _animationPlayer.SetAnimation(_model.MoveAnimation);
            _animationPlayer.SetPosition(_positionUL);

            _initialized = true;
            _isAvailable = false;
        }

        public void Uninitialize()
        {
            _initialized = false;
            _isAvailable = true;
        }

        public XElement Dehydrate()
        {
            XElement xPowerup = new XElement("Powerup",
                new XAttribute("UL-X", _positionUL.X),
                new XAttribute("UL-Y", _positionUL.Y),
                new XAttribute("LR-X", _positionLR.X),
                new XAttribute("LR-Y", _positionLR.Y),
                new XAttribute("Type", Type),
                new XAttribute("State", State),
                new XAttribute("MaxY", _maxY)
                );

            return xPowerup;
        }

        public void Rehydrate(XElement element, PowerupModel model)
        {
            float x = float.Parse(element.Attribute("UL-X").Value);
            float y = float.Parse(element.Attribute("UL-Y").Value);
            Vector2 position = new Vector2(x, y);

            int maxY = int.Parse(element.Attribute("MaxY").Value);

            Initialize(model, ref position, maxY);

            _state = (PowerupState)Enum.Parse(State.GetType(), element.Attribute("State").Value, false);
        }

        public override void Update(GameTime gameTime)
        {
            if (_initialized)
            {
                switch (_state)
                {
                    case PowerupState.Falling:
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
            if (!_initialized || (_state != PowerupState.Falling))
            {
                return;
            }

            _state = PowerupState.PickingUp;
            _model.PickupSound.Play();

            _animationPlayer.SetAnimation(_model.PickupAnimation);
            _animationPlayer.SetPosition(_positionUL);
        }

        private void UpdateDescending()
        {
            if (_positionUL.Y >= _maxY)
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
