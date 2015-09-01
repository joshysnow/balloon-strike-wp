using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameCore;

namespace GameFramework
{
    public class PowerupModel
    {
        public Animation MoveAnimation
        {
            get { return _moveAnimation; }
        }

        public Animation PickupAnimation
        {
            get { return _pickupAnimation; }
        }

        public SoundEffect PickupSound
        {
            get { return _pickupSound; }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
        }

        public Vector2 Size
        {
            get { return _size; }
        }

        private Animation _moveAnimation;
        private Animation _pickupAnimation;
        private SoundEffect _pickupSound;
        private Vector2 _velocity;
        private Vector2 _size;

        public PowerupModel(Animation moveAnimation, Animation pickupAnimation, SoundEffect pickupSound, ref Vector2 velocity)
        {
            _moveAnimation = moveAnimation;
            _pickupAnimation = pickupAnimation;
            _pickupSound = pickupSound;
            _velocity = velocity;

            _size = new Vector2(
                _moveAnimation.FrameWidth * _moveAnimation.Scale,       // Width
                _moveAnimation.FrameHeight * _moveAnimation.Scale);     // Height
        }
    }
}
