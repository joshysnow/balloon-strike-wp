using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum SunMood : byte
    {
        SuperHappy  = 0x01,
        Happy       = 0x02,
        Ok          = 0x04,
        Sad         = 0x08,
        Crying      = 0x0F
    }

    public delegate void SunDeadHandler();

    public class Sun
    {
        public event SunDeadHandler Dead;

        public SunMood Mood
        {
            get;
            private set;
        }

        public bool Alive
        {
            get;
            private set;
        }

        public float MoodPosition
        {
            get
            {
                return _currentLives / _totalLives;
            }
        }

        private const float THRESHOLD_CRYING        = 0.1f;
        private const float THRESHOLD_SAD           = 0.4f;
        private const float THRESHOLD_HAPPY         = 0.7f;
        private const float THRESHOLD_SUPER_HAPPY   = 1f;

        private AnimationPlayer _animationPlayer;
        private Animation _superHappyAnimation;
        private Animation _happyAnimation;
        private Animation _okAnimation;
        private Animation _sadAnimation;
        private Animation _cryingAnimation;
        private Animation _currentAnimation;
        private Vector2 _position;
        private float _totalLives;
        private float _currentLives;

        public Sun()
        {
            _animationPlayer = new AnimationPlayer();
        }

        public void Initialize()
        {
            Alive = true;
            Mood = SunMood.SuperHappy;
            _totalLives = 10;
            _currentLives = 10;
            _position = new Vector2(10, 10);

            ResourceManager resources = ResourceManager.Resources;
            _superHappyAnimation = resources.GetAnimation("sun_superhappy");
            _happyAnimation = resources.GetAnimation("sun_happy");
            _okAnimation = resources.GetAnimation("sun_ok");
            _sadAnimation = resources.GetAnimation("sun_sad");
            _cryingAnimation = resources.GetAnimation("sun_crying");

            _currentAnimation = _superHappyAnimation;
            _animationPlayer.SetAnimation(_currentAnimation);
            _animationPlayer.SetPosition(_position);
        }

        public void LoseALife()
        {
            _currentLives--;

            if (_currentLives < 0)
                _currentLives = 0;

            UpdateMood();
        }

        public void Update(GameTime gameTime)
        {
            if (Mood == SunMood.Crying && _currentLives == 0)
            {
                RaiseDead();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animationPlayer.Draw(spriteBatch);
        }

        private void UpdateMood()
        {
            float moodPercentage = MoodPosition;

            if (moodPercentage <= THRESHOLD_CRYING)
            {
                Mood = SunMood.Crying;
                _currentAnimation = _cryingAnimation;
            }
            else if (moodPercentage <= THRESHOLD_SAD)
            {
                Mood = SunMood.Sad;
                _currentAnimation = _sadAnimation;
            }
            else if((moodPercentage > THRESHOLD_SAD) && (moodPercentage < THRESHOLD_HAPPY))
            {
                Mood = SunMood.Ok;
                _currentAnimation = _okAnimation;
            }
            else if ((moodPercentage >= THRESHOLD_HAPPY) && (moodPercentage < THRESHOLD_SUPER_HAPPY))
            {
                Mood = SunMood.Happy;
                _currentAnimation = _happyAnimation;
            }
            else
            {
                Mood = SunMood.SuperHappy;
                _currentAnimation = _superHappyAnimation;
            }

            _animationPlayer.SetAnimation(_currentAnimation);
        }

        private void RaiseDead()
        {
            if (Dead != null)
                Dead();
        }
    }
}
