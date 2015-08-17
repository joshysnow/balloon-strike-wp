using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameCore;

namespace GameFramework
{
    public class BalloonModel
    {
        public Animation MoveAnimation
        {
            get { return _moveAnimation; }
        }

        public Animation PopAnimation
        {
            get { return _popAnimation; }
        }

        public Animation HitAnimation
        {
            get { return _hitAnimation; }
        }

        public SoundEffect PopSound
        {
            get { return _popSound; }
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
        private Animation _popAnimation;
        private Animation _hitAnimation;
        private SoundEffect _popSound;
        private Vector2 _velocity;
        private Vector2 _size;

        public BalloonModel(Animation move, Animation pop, Animation hit, SoundEffect popSound, ref Vector2 velocity) 
        {
            _moveAnimation = move;
            _popAnimation = pop;
            _hitAnimation = hit;
            _popSound = popSound;
            _velocity = velocity;

            _size = new Vector2(
                _moveAnimation.FrameWidth * _moveAnimation.Scale,       // Width
                _moveAnimation.FrameHeight * _moveAnimation.Scale);     // Height
        }
    }
}
