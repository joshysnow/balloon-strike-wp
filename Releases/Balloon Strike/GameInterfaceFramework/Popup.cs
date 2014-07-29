using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;

namespace GameInterfaceFramework
{
    public class Popup : View
    {
        protected SpriteFont Font
        {
            get;
            private set;
        }

        private Texture2D _background;

        public Popup()
        {
            Transition.TransitionOn = TimeSpan.FromSeconds(1);
            Transition.TransitionOff = TimeSpan.FromSeconds(0.5);
            IsPopup = true;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Font = ResourceManager.Manager.GetFont("popup_text");
                _background = ResourceManager.Manager.GetTexture("blank");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            GraphicsDevice graphics = ViewManager.GraphicsDevice;

            spriteBatch.Begin();
            spriteBatch.Draw(_background, graphics.Viewport.Bounds, Color.Black * (TransitionAlpha / 1.5f));
            spriteBatch.End();
        }
    }
}
