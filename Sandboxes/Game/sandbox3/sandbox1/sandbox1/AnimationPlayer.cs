using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox3
{
    public class AnimationPlayer
    {
        private Animation _animation;
        private Vector2 _position;
        private float _elapsedTime;
        private float _frameTime;
        private int _frameIndexX;
        private int _frameIndexY;
        private bool _animationFinished;

        public bool Finished
        {
            get { return _animationFinished; }
        }

        public AnimationPlayer()
        {
        }

        public void SetAnimation(Animation animation, Vector2 position)
        {
            _animation = animation;
            _position = position;
            _frameIndexX = 0;
            _frameIndexY = 0;
            _elapsedTime = 0;
            _frameTime = 0;
            _animationFinished = false;
        }

        public void UpdateAnimationPosition(Vector2 position)
        {
            _position = position;
        }

        public void Update(GameTime gameTime)
        {
            if (_animationFinished)
            {
                return;
            }

            if (_elapsedTime >= _animation.TotalDuration && !_animation.Loops)
            {
                _elapsedTime = _animation.TotalDuration;
                _animationFinished = true;
            }
            else
            {
                // Add the time passed.
                _elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                _frameTime += gameTime.ElapsedGameTime.Milliseconds;

                // If the current frame has exceeded its duration.
                if (_frameTime >= _animation.FrameDuration)
                {
                    // Advance frame.
                    _frameIndexX = (_frameIndexX + 1) % _animation.HorizontalFrameCount;

                    // Reset the time to next frame.
                    _frameTime %= _animation.FrameDuration;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_animationFinished)
            {
                return;
            }

            Rectangle source = new Rectangle(
                _animation.FrameWidth * _frameIndexX, 
                _animation.FrameHeight * _frameIndexY, 
                _animation.FrameWidth, 
                _animation.FrameHeight);

            spriteBatch.Draw(_animation.AnimationTexture, _position, source, Color.White, 0f, Vector2.Zero, _animation.Scale, SpriteEffects.None, 0f);
        }
    }
}
