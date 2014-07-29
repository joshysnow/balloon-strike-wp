using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameInterfaceFramework;
using GameFramework;

namespace BalloonStrike.Views
{
    public class IntroPopup : Popup
    {
        private string _message;

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _message = "Welcome to the Alpha!";
            }

            base.Activate(instancePreserved);
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            float transitionOffset = (float)Math.Pow(Transition.TransitionPosition, 2);

            Vector2 messageSize = Font.MeasureString(_message);
            Vector2 messagePosition = new Vector2((graphics.Viewport.Width - messageSize.X) / 2, graphics.Viewport.Height / 2);

            if(Transition.State == TransitionState.TransitionOn)
                messagePosition.X -= transitionOffset * (messagePosition.X + messageSize.X);

            if (Transition.State == TransitionState.TransitionOff)
                messagePosition.Y -= transitionOffset * (messagePosition.Y + messageSize.Y);

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, _message, messagePosition, Color.White);// * TransitionAlpha);
            spriteBatch.End();
        }
    }
}
