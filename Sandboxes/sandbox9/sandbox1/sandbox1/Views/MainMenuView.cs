using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;
using GameInterfaceFramework;

namespace Balloonstrike.Views
{
    public class MainMenuView : MenuView
    {


        public MainMenuView()
        {
            
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                // Create all buttons.
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                ResourceManager resources = ResourceManager.Manager;

                Texture2D playButtonTexture = resources.GetTexture("button_play");
                Texture2D aboutButtonTexture = resources.GetTexture("button_about");

                int x = (graphics.Viewport.Width - playButtonTexture.Width) / 2;
                int y = graphics.Viewport.Bounds.Center.Y;

                Button play = new Button(playButtonTexture, new Vector2(x, y));
                play.Tapped += PlayButtonTapped;
                _menuButtons.Add(play);

                y += (int)(playButtonTexture.Height * 1.5f);

                Button about = new Button(aboutButtonTexture, new Vector2(x, y));
                about.Tapped += AboutButtonTapped;
                _menuButtons.Add(about);
            }
        }

        private void PlayButtonTapped(Button button)
        {
            Exit();
            LoadView.Load(ViewManager, 1, new GameView());
        }

        private void AboutButtonTapped(Button button)
        {
            Exit();
            LoadView.Load(ViewManager, 1, new AboutView());
        }
    }
}
