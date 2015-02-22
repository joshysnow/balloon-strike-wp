using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameInterfaceFramework;
using GameInterfaceFramework.Actions;

namespace BalloonStrike.Views
{
    public class InputPopup : Popup
    {
        private IActionHandler _yesAction;
        private IActionHandler _noAction;

        public InputPopup(string title, string message, IActionHandler acceptAction)
            : base(title, message)
        {
            _yesAction = acceptAction;
        }

        public InputPopup(string title, string message, IActionHandler acceptAction, IActionHandler cancelAction) 
            : base(title, message) 
        {
            _yesAction = acceptAction;
            _noAction = cancelAction;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ResourceManager resources = ResourceManager.Resources;
                Texture2D yesTexture = resources.GetTexture("button_yes");
                Texture2D noTexture = resources.GetTexture("button_no");

                Button yesButton = new Button(yesTexture);
                yesButton.Tapped += YesButtonTappedHandler;
                _menuButtons.Add(yesButton);

                Button noButton = new Button(noTexture);
                noButton.Tapped += NoButtonTappedHandler;
                _menuButtons.Add(noButton);
            }

            base.Activate(instancePreserved);
        }

        public override void Draw(GameTime gameTime)
        {
            SetYesButtonPosition();
            SetNoButtonPosition();

            base.Draw(gameTime);
        }

        private void SetYesButtonPosition()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            Button yesButton = _menuButtons.First();

            float buttonX = (graphics.Viewport.Width / 2) - (BUTTON_HORIZONTAL_SPACING / 2) - yesButton.Size.X;
            float buttonY = (ForegroundPosition + ForegroundSize).Y + GUI_SPACING;

            Vector2 buttonPositon = new Vector2(buttonX, buttonY);
            yesButton.Position = GetTransitionPosition(buttonPositon);
        }

        private void SetNoButtonPosition()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            Button noButton = _menuButtons.Last();

            float buttonX = (graphics.Viewport.Width / 2) + (BUTTON_HORIZONTAL_SPACING / 2);
            float buttonY = (ForegroundPosition + ForegroundSize).Y + GUI_SPACING;

            Vector2 buttonPositon = new Vector2(buttonX, buttonY);
            noButton.Position = GetTransitionPosition(buttonPositon);
        }

        private void YesButtonTappedHandler(Button button)
        {
            _yesAction.Execute();

            if(!IsExiting)
                Exit();
        }

        private void NoButtonTappedHandler(Button button)
        {
            if (_noAction != null)
                _noAction.Execute();

            if (!IsExiting)
                Exit();
        }
    }
}
