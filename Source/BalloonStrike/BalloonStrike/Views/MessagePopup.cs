using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class MessagePopup : Popup
    {
        public MessagePopup(string title, string message) : base(title, message) { }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;
                ResourceManager resources = ResourceManager.Resources;
                Texture2D okTexture = resources.GetTexture("button_ok");

                Button okButton = new Button(okTexture);
                okButton.Tapped += OkTappedHandler;
                _menuButtons.Add(okButton);
            }

            base.Activate(instancePreserved);
        }

        public override void Draw(GameTime gameTime)
        {
            SetButtonPosition();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates the button position as it transitions across the screen.
        /// </summary>
        private void SetButtonPosition()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            Button okButton = _menuButtons.First();

            float buttonX = (graphics.Viewport.Width - okButton.Size.X) / 2;
            float buttonY = (ForegroundPosition + ForegroundSize).Y + GUI_SPACING;

            Vector2 buttonPositon = new Vector2(buttonX, buttonY);
            okButton.Position = GetTransitionPosition(buttonPositon);
        }

        private void OkTappedHandler(Button button)
        {
            Exit();
        }
    }
}
