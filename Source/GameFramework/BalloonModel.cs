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

        public float StartHealth
        {
            get { return _health; }
        }

        private Animation _moveAnimation;
        private Animation _popAnimation;
        private Animation _hitAnimation;
        private SoundEffect _popSound;
        private Vector2 _velocity;
        private float _health;

        public BalloonModel(Animation move, Animation pop, Animation hit, SoundEffect popSound, ref Vector2 velocity, float health) 
        {
            _moveAnimation = move;
            _popAnimation = pop;
            _hitAnimation = hit;
            _popSound = popSound;
            _velocity = velocity;
            _health = health;
        }
    }
}
