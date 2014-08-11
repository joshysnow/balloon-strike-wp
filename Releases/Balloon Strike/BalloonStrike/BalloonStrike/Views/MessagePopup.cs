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
        private string _message;

        public MessagePopup(string message)
            : base()
        {
            _message = message;
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                ResourceManager resources = ResourceManager.Resources;
                Texture2D okUnselected = resources.GetTexture("button_unselected_tick");
                Texture2D okSelected = resources.GetTexture("button_selected_tick");
                Button okButton = new Button(okUnselected, okSelected);
                okButton.Tapped += OkTappedHandler;
                _menuButtons.Add(okButton);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            Vector2 messageSize = Font.MeasureString(_message);
            Vector2 messagePosition = new Vector2((graphics.Viewport.Width - messageSize.X) / 2, (graphics.Viewport.Height - messageSize.Y) / 2);
            TransitionPosition(ref messageSize, ref messagePosition);

            const byte SPACING = 20;
            Button okButton = _menuButtons.First();
            Vector2 buttonSize = okButton.Size;
            Vector2 buttonPositon = new Vector2((graphics.Viewport.Width - buttonSize.X) / 2,
                    (ForegroundPosition + ForegroundSize).Y - SPACING - buttonSize.Y);
            TransitionPosition(ref buttonSize, ref buttonPositon);
            okButton.Origin = buttonPositon;

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, _message, messagePosition, Color.White);
            okButton.Draw(this);
            spriteBatch.End();
        }

        private void OkTappedHandler(Button button)
        {
            Exit();
        }
    }
}
