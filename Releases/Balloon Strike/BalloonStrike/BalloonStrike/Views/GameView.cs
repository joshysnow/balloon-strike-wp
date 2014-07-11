using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class GameView : View
    {
        public GameView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
            else
            {
                if (controls.Gestures.Length > 0)
                {
                    Exit();
                    LoadView.Load(ViewManager, 1, new GameOverView());
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ViewManager.GraphicsDevice.Clear(Color.Violet * TransitionAlpha);
        }
    }
}
