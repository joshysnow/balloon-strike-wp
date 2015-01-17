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
        public MessagePopup(string message) : base(message) { }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                ResourceManager resources = ResourceManager.Resources;
                //Texture2D okUnselected = resources.GetTexture("button_unselected_tick");
                //Texture2D okSelected = resources.GetTexture("button_selected_tick");
                Texture2D okTexture = resources.GetTexture("button_oklong");

                Button okButton = new Button(okTexture);
                okButton.Tapped += OkTappedHandler;
                _menuButtons.Add(okButton);
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            UpdateButton();

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            spriteBatch.Begin();

            Button okButton = _menuButtons.First();
            okButton.Draw(this);

            spriteBatch.End();
        }

        /// <summary>
        /// Updates the buttons position as it transitions across the screen.
        /// </summary>
        private void UpdateButton()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;

            Button okButton = _menuButtons.First();
            Vector2 buttonSize = okButton.Size;
            Vector2 buttonPositon = new Vector2(
                (graphics.Viewport.Width - buttonSize.X) / 2,
                (ForegroundPosition + ForegroundSize).Y + BUTTON_VERTICAL_SPACING);

            TransitionPosition(ref buttonSize, ref buttonPositon);
            okButton.Position = buttonPositon;
        }

        private void OkTappedHandler(Button button)
        {
            Exit();
        }
    }
}
