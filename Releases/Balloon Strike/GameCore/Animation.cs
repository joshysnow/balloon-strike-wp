using System;
using Microsoft.Xna.Framework.Graphics;

namespace GameCore
{
    public class Animation
    {
        private Texture2D _animation;
        private float _animationDuration;
        private float _frameDuration;
        private float _scale;
        private int _frameCount;
        private int _verticalFrames;
        private int _horizontalFrames;
        private int _frameWidth;
        private int _frameHeight;
        private bool _loop;

        /// <summary>
        /// The texture that contains all the frames.
        /// </summary>
        public Texture2D AnimationTexture
        {
            get { return _animation; }
        }

        /// <summary>
        /// Total time the animation runs for in ms.
        /// </summary>
        public float TotalDuration
        {
            get { return _animationDuration; }
        }

        /// <summary>
        /// Time each frame should be on screen for in ms.
        /// </summary>
        public float FrameDuration
        {
            get { return _frameDuration; }
        }

        /// <summary>
        /// Total frames in this animation.
        /// </summary>
        public int FrameCount
        {
            get { return _frameCount; }
        }

        /// <summary>
        /// Total frames across the X axis of the animation.
        /// </summary>
        public int HorizontalFrameCount
        {
            get { return _horizontalFrames; }
        }

        /// <summary>
        /// Total frames down the Y axis of the animation.
        /// </summary>
        public int VerticalFrameCount
        {
            get { return _verticalFrames; }
        }

        /// <summary>
        /// The width in pixels of each frame.
        /// </summary>
        public int FrameWidth
        {
            get { return _frameWidth; } 
        }

        /// <summary>
        /// The height in pixels of each frame.
        /// </summary>
        public int FrameHeight
        {
            get { return _frameHeight; }
        }

        /// <summary>
        /// True if the animation should loop.
        /// </summary>
        public bool Loops
        {
            get { return _loop; }
        }

        /// <summary>
        /// As a percentage how much bigger or smaller the animation should be.
        /// </summary>
        public float Scale
        {
            get { return _scale; }
        }

        /// <summary>
        /// Construct a new animation object.
        /// </summary>
        /// <param name="animation">The texture for the animation.</param>
        /// <param name="loops">True if the animation should loop.</param>
        /// <param name="frameWidth">The width per frame in pixels relative to the size on unscaled texture.</param>
        /// <param name="frameHeight">The height of each frame in pixels relative to the size on unscaled texture.</param>
        /// <param name="duration">Total running time.</param>
        /// <param name="scale">How big or small the animation should be drawn as a percentage.</param>
        public Animation(Texture2D animation, bool loops, int frameWidth, int frameHeight, float duration, float scale)
        {
            _animation = animation;
            _loop = loops;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _animationDuration = duration;
            _horizontalFrames = (frameWidth / animation.Width);
            _verticalFrames = (frameHeight / animation.Height);
            _frameCount = _horizontalFrames * _verticalFrames;
            _frameDuration = _animationDuration / _frameCount;
            _scale = scale;
        }
    }
}