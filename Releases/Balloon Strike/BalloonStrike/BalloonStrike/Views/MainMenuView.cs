using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;
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
                ResourceManager resources = ResourceManager.Manager;

                Texture2D playUnselected = resources.GetTexture("button_unselected_play");
                Texture2D infoUnselected = resources.GetTexture("button_unselected_info");
                Texture2D playSelected = resources.GetTexture("button_selected_play");
                Texture2D infoSelected = resources.GetTexture("button_selected_info");

                int x = (graphics.Viewport.Width - playUnselected.Width) / 2;
                int y = graphics.Viewport.Bounds.Center.Y;

                Button play = new Button(playUnselected, playSelected) { Origin = new Vector2(x, y) };
                play.Tapped += PlayButtonTapped;
                _menuButtons.Add(play);

                y += (int)(playUnselected.Height * 1.5f);

                Button infoButton = new Button(infoUnselected, infoSelected) { Origin = new Vector2(x, y) };
                infoButton.Tapped += AboutButtonTapped;
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

        private void PlayButtonTapped(Button button)
        {
            LoadView.Load(ViewManager, 1, new GameView());
        }

        private void AboutButtonTapped(Button button)
        {
            LoadView.Load(ViewManager, 1, new InfoView());
        }
    }
}
