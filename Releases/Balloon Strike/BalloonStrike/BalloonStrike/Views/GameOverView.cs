using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class GameOverView : View
    {
        public GameOverView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed() || controls.Gestures.Length > 0)
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ViewManager.GraphicsDevice.Clear(Color.Red * TransitionAlpha);
        }
    }
}
