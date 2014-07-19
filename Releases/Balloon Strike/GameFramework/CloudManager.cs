using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFramework
{
    public class CloudManager : CharacterManager
    {
        public CloudManager(GraphicsDevice graphics, TriggerManager triggers) : base(graphics, triggers) { }

        public override void UpdatePlayerInput(Microsoft.Xna.Framework.Input.Touch.GestureSample[] gestures, Weapon currentWeapon, out Microsoft.Xna.Framework.Input.Touch.GestureSample[] remainingGestures)
        {
            // Do nothing, clouds cannot be tapped.
            remainingGestures = gestures;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateCharacters(GameTime gameTime)
        {
            foreach (Cloud cloud in Characters)
            {
                if (cloud.State == CloudState.TransitionOff)
                {
                    // TODO: Remove or add somewhere.
                }
                else
                {
                    cloud.Update(gameTime);
                }
            }
        }
    }
}
