using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class MainMenuView : MenuView
    {
        private Texture2D _titleTexture;
        private Vector2 _titlePosition;
        private SpriteFont _versionFont;
        private Vector2 _versionPosition;
        private string _versionText;

        public MainMenuView(bool rehydrated = false)
            :base(rehydrated)
        {
            _versionText = "VERSION UNDETECTED";
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                // Create all buttons.
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                ResourceManager resources = ResourceManager.Resources;

                Texture2D playTexture = resources.GetTexture("button_play");
                Texture2D highscoresTexture = resources.GetTexture("button_highscores");
                Texture2D aboutTexture = resources.GetTexture("button_about");
                Texture2D exitTexture = resources.GetTexture("button_exit");

                const int BUTTON_SPACING    = 20;
                const int NUM_BUTTONS       = 4;
                const int BOTTOM_SPACING    = 60;  // Bottom of the screen space between buttons (pixels).

                int x = (graphics.Viewport.Width - playTexture.Width) / 2;
                int y = graphics.Viewport.Height - ((playTexture.Height * NUM_BUTTONS) + (BUTTON_SPACING * NUM_BUTTONS) + BOTTOM_SPACING);

                Button play = new Button(playTexture, playTexture) { Position = new Vector2(x, y) };
                play.Tapped += PlayTappedHandler;
                _menuButtons.Add(play);
                y += BUTTON_SPACING + playTexture.Height;

                Button highscores = new Button(highscoresTexture, highscoresTexture) { Position = new Vector2(x, y) };
                highscores.Tapped += HighscoresTappedHandler;
                _menuButtons.Add(highscores);
                y += BUTTON_SPACING + highscoresTexture.Height;

                Button about = new Button(aboutTexture, aboutTexture) { Position = new Vector2(x, y) };
                about.Tapped += InfoTappedHandler;
                _menuButtons.Add(about);
                y += BUTTON_SPACING + aboutTexture.Height;

                Button exit = new Button(exitTexture, exitTexture) { Position = new Vector2(x, y) };
                exit.Tapped += ExitTappedHandler;
                _menuButtons.Add(exit);

                SetupTitle();
                SetupVersion();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(_titleTexture, _titlePosition, null, Color.White * TransitionAlpha);
            spriteBatch.DrawString(_versionFont, _versionText, _versionPosition, Color.Black * TransitionAlpha);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetupTitle()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            ResourceManager resources = ResourceManager.Resources;

            _titleTexture = resources.GetTexture("title");
            int x = (graphics.Viewport.Width - _titleTexture.Width) / 2;
            int y = ((graphics.Viewport.Height / 2) - _titleTexture.Height) / 2;
            _titlePosition = new Vector2(x, y);
        }

        private void SetupVersion()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            ResourceManager resources = ResourceManager.Resources;

            // Show the version in the corner of the screen.
            _versionFont = resources.GetFont("small");
            const byte VERSION_INDEX = 1;
            const byte SPACING = 10;

            string[] info = Assembly.GetExecutingAssembly().FullName.Split(',');
            string[] versionInfo = info[VERSION_INDEX].Trim().Split('=')[VERSION_INDEX].Split('.');
            _versionText = 'v' + versionInfo[0] + '.' + versionInfo[1];

            Vector2 versionSize = _versionFont.MeasureString(_versionText);
            _versionPosition = new Vector2(graphics.Viewport.Width - versionSize.X - SPACING, graphics.Viewport.Height - versionSize.Y);
        }

        private void PlayTappedHandler(Button button)
        {
            // Setup a new game.
            LoadView.Load(ViewManager, 1, new GameView());
        }

        private void HighscoresTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new HighscoreView());
        }

        private void InfoTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new InfoView());
        }

        private void ExitTappedHandler(Button button)
        {
            ViewManager.Game.Exit();
        }
    }
}
