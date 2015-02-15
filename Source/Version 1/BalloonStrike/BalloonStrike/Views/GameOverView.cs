using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
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
        private bool _finishedCounting;

        public GameOverView()
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);

            ViewGestures = GestureType.None;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                int screenWidth = graphics.Viewport.Width;
                int screenHeight = graphics.Viewport.Height;

                ResourceManager resources = ResourceManager.Resources;
                _score = 0;
                _scoreFont = resources.GetFont("score");

#warning TODO: Calculate the central position for the final score using the score manager. 
#warning This is so the score won't move from right to left as the digits increase from tens, to hundreds etc. (regarding positioning)

                _titleFont = ResourceManager.Resources.GetFont("your_score");
                Vector2 titleSize = _titleFont.MeasureString(YOUR_SCORE);
                _titlePosition = new Vector2((screenWidth - titleSize.X) / 2, (screenHeight / 2) - titleSize.Y);

                Player p = Player.Instance;
                Vector2 scoreSize = _scoreFont.MeasureString(p.CurrentScore.ToString());
                _scorePosition = new Vector2((screenWidth - scoreSize.X) / 2, (screenHeight / 2));

                // If the player scored 0, then we have already finished counting.
                _finishedCounting = (p.CurrentScore == 0);

                // Ensure that if the score would take longer than usual then limit to 1/2 a second. In other words,
                // if the score would take more than 1/2 a second to increment by one, then calculate the above to ensure
                // it takes no longer than 1/2 a second.

                // Calculate the amount to increment for large scores.
                if (_finishedCounting == false)
                    _increment = (33.3f / 500) * p.CurrentScore; // Finish in 1/2 a second. (FPS / time to finish) * score.
                else
                    EnableViewGestures();

                Texture2D playTexture = resources.GetTexture("button_playagain");
                Texture2D mainMenuTexture = resources.GetTexture("button_mainmenu");

                const int BUTTON_HORIZONTAL_SPACING = 10;

                int y = (screenHeight - (screenHeight / 4)) - (playTexture.Height / 2);
                int x = (screenWidth / 2) - BUTTON_HORIZONTAL_SPACING - playTexture.Width;

                Button playAgain = new Button(playTexture) { 
                    Position = new Vector2(x, y) 
                };

                playAgain.Tapped += PlayTappedHandler;
                _menuButtons.Add(playAgain);

                x = (screenWidth / 2) + BUTTON_HORIZONTAL_SPACING;

                Button mainMenu = new Button(mainMenuTexture) { 
                    Position = new Vector2(x, y) 
                };

                mainMenu.Tapped += MenuTappedHandler;
                _menuButtons.Add(mainMenu);
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (State == TransitionState.Active && !_finishedCounting)
            {
                Player p = Player.Instance;
                if (_score < p.CurrentScore)
                {
                    if ((_score + _increment) >= p.CurrentScore)
                    {
                        _score = p.CurrentScore;
                        _finishedCounting = true;
                        EnableViewGestures();
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
            
            spriteBatch.DrawString(_titleFont, YOUR_SCORE, _titlePosition, Color.Black * TransitionAlpha);
            spriteBatch.DrawString(_scoreFont, ((int)_score).ToString(), _scorePosition, Color.Black * TransitionAlpha);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void HandleBackButtonPressed()
        {
            LoadView.Load(ViewManager, 1, new MainMenuView());
        }

        private void PlayTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new GameView());
        }

        private void MenuTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new MainMenuView());
        }

        private void EnableViewGestures()
        {
            ViewGestures = GestureType.Tap;
            ViewManager.EnabledGestures = ViewGestures;
        }
    }
}
