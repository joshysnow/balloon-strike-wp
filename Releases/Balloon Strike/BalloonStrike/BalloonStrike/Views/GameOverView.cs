using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class GameOverView : MenuView
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
            Transition.TransitionOn = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOff = TimeSpan.FromSeconds(0.5);

            EnabledGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;

                ResourceManager resources = ResourceManager.Manager;
                _score = 0;
                _scoreFont = resources.GetFont("score");

                // TODO: Calculate the central position for the final score using the score manager. This is so the score
                // won't move from right to left as the digits increase from tens, to hundreds etc.
                _titleFont = ResourceManager.Manager.GetFont("your_score");
                Vector2 titleSize = _titleFont.MeasureString(YOUR_SCORE);
                _titlePosition = new Vector2((width - titleSize.X) / 2, (height / 2) - titleSize.Y);

                Player p = Player.Instance;
                Vector2 scoreSize = _scoreFont.MeasureString(p.CurrentScore.ToString());
                _scorePosition = new Vector2((width - scoreSize.X) / 2, (height / 2));


                // Calculate the amount to increment for large scores.
                _increment = (33.3f / 500) * p.CurrentScore; // Finish in 1/2 a second. (FPS / time to finish) * score.
                // TODO: Ensure that if the score would take longer than usual then limit to 1/2 a second. In other words,
                // if the score would take more than 1/2 a second to increment by one, then calculate the above to ensure
                // it takes no longer than 1/2 a second.

                Texture2D playAgainTexture = resources.GetTexture("button_playagain");
                Texture2D menuTexture = resources.GetTexture("button_mainmenu");

                int y = (height - (height / 4)) - (playAgainTexture.Height / 2);
                int spacing = 7;

                Button playAgain = new Button(playAgainTexture, new Vector2(((width / 2) - spacing) - playAgainTexture.Width, y));
                playAgain.Tapped += PlayTappedHandler;
                _menuButtons.Add(playAgain);

                Button mainMenu = new Button(menuTexture, new Vector2((width / 2) + spacing, y));
                mainMenu.Tapped += MenuTappedHandler;
                _menuButtons.Add(mainMenu);
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (State == TransitionState.Active)
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

            base.Draw(gameTime);
        }

        protected override void HandleBackButtonPressed()
        {
            Exit();
            LoadView.Load(ViewManager, 1, new MainMenuView());
        }

        private void PlayTappedHandler(Button button)
        {
            Exit();
            LoadView.Load(ViewManager, 1, new GameView());
        }

        private void MenuTappedHandler(Button button)
        {
            Exit();
            LoadView.Load(ViewManager, 1, new MainMenuView());
        }
    }
}
