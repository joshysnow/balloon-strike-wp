using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using GameFramework;
using GameFramework.Triggers;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class BackgroundView : View
    {
        //private Song _music;

        private CloudManager _clouds;

        public BackgroundView(bool rehydrated = false)
            :base(rehydrated)
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);

            IsSerializable = true;
        }

        public override void Activate(bool instancePreserved)
        {
            // Instantiate for the first time or rehydrate.
            if (!instancePreserved)
                _clouds = new CloudManager(ViewManager.GraphicsDevice);
                _clouds.Activate(instancePreserved, !Rehydrated);
            //_music = ResourceManager.Manager.GetSong("test");

            base.Activate(instancePreserved);
        }

        public override void Deactivate()
        {
            // Sierialize the cloud manager.
            _clouds.Deactivate();

            base.Deactivate();
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            //if (MediaPlayer.State == MediaState.Stopped || MediaPlayer.State == MediaState.Paused)
            //{
            //    MediaPlayer.Play(_music);
            //}

            _clouds.Update(gameTime);

            // Background will always be covered but we would like it to update still.
            base.Update(gameTime, false);
        }

        public override void Draw(GameTime gameTime)
        {
            ViewManager.GraphicsDevice.Clear(Color.SkyBlue);

            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            spriteBatch.Begin();
            _clouds.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
