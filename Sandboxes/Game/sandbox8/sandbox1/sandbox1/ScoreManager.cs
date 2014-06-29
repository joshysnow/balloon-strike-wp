using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox8
{
    public class ScoreManager
    {
        public int Score
        {
            get
            {
                return _score;
            }
        }

        private SpriteFont[] _fonts;
        private TimeSpan _transitionTime;
        private float _position;
        private int _direction;
        private int _score;
        private bool _transition;

        public ScoreManager()
        {
            LoadFonts();

            _transitionTime = TimeSpan.FromSeconds(1f);
            _position = 0;
            _direction = 1;
            _score = 0;
            _transition = false;
        }

        public void Reset()
        {
            _position = 0;
            _direction = 1;
            _score = 0;
        }

        public void IncreaseScore(int amount)
        {
            _score += amount;

            // Reset animation.
            _position = 0;
            _direction = 1;
            _transition = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_transition)
            {
                // How much to move by. Time for transition halved as the animation is played twice (once backwards).
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / (_transitionTime.TotalMilliseconds / 2));

                // Increase or decrease position.
                _position += delta * _direction;
                _position = MathHelper.Clamp(_position, 0, 1);

                if ((_direction == 1) && (_position == 1))
                {
                    _direction = -1;
                }

                if ((_direction == -1) && (_position <= (1 / 8192)))
                {
                    _position = 0;
                    _direction = 1;
                    _transition = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteFont font = _fonts[(int)(_position * 9)];
            Vector2 scoreLength = font.MeasureString(_score.ToString());
            Vector2 position = new Vector2(480 - scoreLength.X - 10, 0); // Screen width - horizontal length of text - spacing from the side, top margin.

            spriteBatch.DrawString(font, _score.ToString(), position, Color.Goldenrod);
        }

        private void LoadFonts()
        {
            _fonts = new SpriteFont[10];

            ResourceManager manager = ResourceManager.Manager;

            for (int i = 0; i < _fonts.Length; i++)
            {
                _fonts[i] = manager.GetFont("font_score" + i);
            }
        }
    }
}
