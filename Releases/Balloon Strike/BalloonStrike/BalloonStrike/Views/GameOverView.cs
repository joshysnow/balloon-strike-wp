using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class GameOverView : View
    {
        private const string YOUR_SCORE = "YOUR SCORE";
        private SpriteFont _titleFont;
        private SpriteFont _scoreFont;
        private Vector2 _titlePosition;
        private Vector2 _scorePosition;
        private float _increment;
        private float _score;

        public GameOverView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _score = 0;
                _scoreFont = ResourceManager.Manager.GetFont("score");

                // TODO: Calculate the central position for the final score using the score manager. This is so the score
                // won't move from right to left as the digits increase from tens, to hundreds etc.
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                Player p = Player.Instance;
                Vector2 scoreSize = _scoreFont.MeasureString(p.CurrentScore.ToString());
                _scorePosition = new Vector2((graphics.Viewport.Width - scoreSize.X) / 2, (graphics.Viewport.Height + scoreSize.Y) / 2);

                _titleFont = ResourceManager.Manager.GetFont("your_score");
                Vector2 titleSize = _titleFont.MeasureString(YOUR_SCORE);
                _titlePosition = new Vector2((graphics.Viewport.Width - titleSize.X) / 2, ((graphics.Viewport.Height / 2) + titleSize.Y) / 2);


                // Calculate the amount to increment for large scores.
                _increment = (33.3f / 500) * p.CurrentScore; // Finish in 1/2 a second. (FPS / time to finish) * score.
                // TODO: Ensure that if the score would take longer than usual then limit to 1/2 a second. In other words,
                // if the score would take more than 1/2 a second to increment by one, then calculate the above to ensure
                // it takes no longer than 1/2 a second.
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (State == ViewState.Active)
            {
                Player p = Player.Instance;
                if (_score < p.CurrentScore)
                {
                    if ((_score + _increment) >= p.CurrentScore)
                    {
                        _score = p.CurrentScore;
                    }
                    else
                    {
                        _score += _increment;
                    }
                }
            }

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            spriteBatch.Begin();
            
            spriteBatch.DrawString(_titleFont, YOUR_SCORE, _titlePosition, Color.White * TransitionAlpha);
            spriteBatch.DrawString(_scoreFont, ((int)_score).ToString(), _scorePosition, Color.White * TransitionAlpha);

            spriteBatch.End();
        }
    }
}
