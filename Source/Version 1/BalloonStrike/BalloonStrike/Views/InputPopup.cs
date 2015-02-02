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
        private IActionHandler _acceptAction;
        private IActionHandler _cancelAction;

        public InputPopup(string message, IActionHandler acceptAction)
            : base(message)
        {
            _acceptAction = acceptAction;
        }

        public InputPopup(string message, IActionHandler acceptAction, IActionHandler cancelAction) 
            : base(message) 
        {
            _acceptAction = acceptAction;
            _cancelAction = cancelAction;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ResourceManager resources = ResourceManager.Resources;
                Texture2D acceptUnselected = resources.GetTexture("button_unselected_tick");
                Texture2D acceptSelected = resources.GetTexture("button_selected_tick");

                Texture2D cancelUnselected = resources.GetTexture("button_unselected_cancel");
                Texture2D cancelSelected = resources.GetTexture("button_selected_cancel");

                Button acceptButton = new Button(acceptUnselected, acceptSelected);
                acceptButton.Tapped += AcceptButtonTappedHandler;
                _menuButtons.Add(acceptButton);

                Button cancelButton = new Button(cancelUnselected, cancelSelected);
                cancelButton.Tapped += CancelButtonTappedHandler;
                _menuButtons.Add(cancelButton);
            }

            base.Activate(instancePreserved);
        }

        public override void Draw(GameTime gameTime)
        {
            SetAcceptButtonPosition();
            SetCancelButtonPosition();

            base.Draw(gameTime);
        }

        private void SetAcceptButtonPosition()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            Button acceptButton = _menuButtons.First();

            float buttonX = (graphics.Viewport.Width / 4) - (acceptButton.Size.X / 2);
            float buttonY = (ForegroundPosition + ForegroundSize).Y + BUTTON_VERTICAL_SPACING;

            Vector2 buttonPositon = new Vector2(buttonX, buttonY);
            acceptButton.Position = GetTransitionPosition(buttonPositon);
        }

        private void SetCancelButtonPosition()
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;
            Button cancelButton = _menuButtons.Last();

            float buttonX = graphics.Viewport.Width - (graphics.Viewport.Width / 4) - (cancelButton.Size.X / 2);
            float buttonY = (ForegroundPosition + ForegroundSize).Y + BUTTON_VERTICAL_SPACING;

            Vector2 buttonPositon = new Vector2(buttonX, buttonY);
            cancelButton.Position = GetTransitionPosition(buttonPositon);
        }

        private void AcceptButtonTappedHandler(Button button)
        {
            _acceptAction.Execute();

            if(!IsExiting)
                Exit();
        }

        private void CancelButtonTappedHandler(Button button)
        {
            if (_cancelAction != null)
                _cancelAction.Execute();

            if (!IsExiting)
                Exit();
        }
    }
}
