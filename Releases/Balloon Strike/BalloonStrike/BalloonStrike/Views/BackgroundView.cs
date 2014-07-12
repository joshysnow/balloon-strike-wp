using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class BackgroundView : View
    {
        //private Texture2D _backgroundTexture;
        //private Song _music;

        public BackgroundView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved)
        {
            //_backgroundTexture = ResourceManager.Manager.GetTexture("splash");
            if (!instancePreserved)
            {
                //_music = ResourceManager.Manager.GetSong("test");
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            //if (MediaPlayer.State == MediaState.Stopped || MediaPlayer.State == MediaState.Paused)
            //{
            //    MediaPlayer.Play(_music);
            //}

            // Background will always be covered but we would like it to update still.
            base.Update(gameTime, false);
        }

        public override void Draw(GameTime gameTime)
        {
            //SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            //spriteBatch.Begin();
            //spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White * TransitionAlpha);
            //spriteBatch.End();

            ViewManager.GraphicsDevice.Clear(Color.SkyBlue);
        }
    }
}
