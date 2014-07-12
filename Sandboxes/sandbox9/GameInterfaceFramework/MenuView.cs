using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameInterfaceFramework
{
    public class MenuView : View
    {
        protected List<Button> _menuButtons;

        public MenuView()
        {
            _menuButtons = new List<Button>();

            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                HandleBackButtonPressed();
            }
            else
            {
                GestureSample[] gestures = controls.Gestures;

                foreach (GestureSample gesture in gestures)
                {
                    if (gesture.GestureType != GestureType.Tap)
                    {
                        continue;
                    }

                    foreach (Button button in _menuButtons)
                    {
                        if (button.HandleTap(gesture.Position))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ViewManager.SpriteBatch.Begin();
            foreach (Button button in _menuButtons)
            {
                button.Draw(this);
            }
            ViewManager.SpriteBatch.End();
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        protected virtual void HandleBackButtonPressed()
        {
            ViewManager.Game.Exit();
        }
    }
}