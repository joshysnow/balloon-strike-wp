using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;
using GameInterfaceFramework;

namespace Balloonstrike.Views
{
    public class GameView : View
    {

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ViewManager.GraphicsDevice.Clear(Color.Violet);

        }
    }
}
