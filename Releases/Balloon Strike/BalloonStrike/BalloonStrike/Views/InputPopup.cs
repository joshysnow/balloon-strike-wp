using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class InputPopup : Popup
    {


        public InputPopup(string message)
            : base()
        {

        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                ResourceManager resources = ResourceManager.Resources;
                Texture2D yesUnselected = resources.GetTexture("button_unselected_tick");
                Texture2D cancelUnselected = resources.GetTexture("button_unselected_cancel");
                Texture2D yesSelected = resources.GetTexture("button_selected_tick");
                Texture2D cancelSelected = resources.GetTexture("button_selected_cancel");

                Button yesButton = new Button(yesUnselected, yesSelected);
                yesButton.Tapped += YesButtonTappedHandler;
                Button noButton = new Button(cancelUnselected, yesUnselected);
            }
        }

        private void YesButtonTappedHandler(Button button)
        {
            
        }
    }
}
