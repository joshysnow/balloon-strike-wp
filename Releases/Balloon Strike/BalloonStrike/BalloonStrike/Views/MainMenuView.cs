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
        private SpriteFont _versionFont;
        private Vector2 _versionPosition;
        private string _versionText;

        public MainMenuView() : base()
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

                Texture2D playUnselected = resources.GetTexture("button_unselected_play");
                Texture2D achieveUnselected = resources.GetTexture("button_unselected_achievements");
                Texture2D infoUnselected = resources.GetTexture("button_unselected_info");
                Texture2D playSelected = resources.GetTexture("button_selected_play");
                Texture2D achieveSelected = resources.GetTexture("button_selected_achievements");
                Texture2D infoSelected = resources.GetTexture("button_selected_info");

                const int BUTTON_SPACING = 10;
                int x = (graphics.Viewport.Width - playUnselected.Width) / 2;
                int y = graphics.Viewport.Height - ((playUnselected.Height * 3) + (BUTTON_SPACING * 3));

                Button play = new Button(playUnselected, playSelected) { Position = new Vector2(x, y) };
                play.Tapped += PlayTappedHandler;
                _menuButtons.Add(play);
                y += BUTTON_SPACING + playUnselected.Height;

                Button achievements = new Button(achieveUnselected, achieveSelected) { Position = new Vector2(x, y) };
                achievements.Tapped += AchievementsTappedHandler;
                _menuButtons.Add(achievements);
                y += BUTTON_SPACING + playUnselected.Height;

                Button infoButton = new Button(infoUnselected, infoSelected) { Position = new Vector2(x, y) };
                infoButton.Tapped += InfoTappedHandler;
                _menuButtons.Add(infoButton);

                // Show the version in the corner of the screen.
                _versionFont = resources.GetFont("small");
                const byte VERSION = 1;
                const byte SPACING = 10;
                string[] info = Assembly.GetExecutingAssembly().FullName.Split(',');
                _versionText = "v" + info[VERSION].Trim().Split('=')[VERSION];
                Vector2 versionSize = _versionFont.MeasureString(_versionText);
                _versionPosition = new Vector2(graphics.Viewport.Width - versionSize.X - SPACING, graphics.Viewport.Height - versionSize.Y);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(_versionFont, _versionText, _versionPosition, Color.Black * TransitionAlpha);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void PlayTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new GameView());
        }

        private void AchievementsTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new AchievementsView());
        }

        private void InfoTappedHandler(Button button)
        {
            LoadView.Load(ViewManager, 1, new InfoView());
        }
    }
}
