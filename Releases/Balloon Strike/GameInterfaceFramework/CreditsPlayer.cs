using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameInterfaceFramework
{
    public class CreditsPlayer
    {
        private SpriteFont _titleFont;
        private SpriteFont _nameFont;
        private List<Credit> _credits;
        private Transition _transition;
        private byte _index;

        public CreditsPlayer()
        {
            _credits = new List<Credit>();
            _transition = new Transition();
            _index = 0;
        }

        public void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
#warning TODO: Load XML for credits?
                _titleFont = null;
            }
        }

        public void AddCredit(params Credit[] credits)
        {
            if (credits != null)
            {
                foreach (Credit c in credits)
                {
                    _credits.Add(c);   
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_transition.State == TransitionState.Active)
            {
                _transition.State = TransitionState.TransitionOff;
            }

            if (_transition.State == TransitionState.Hidden)
            {
                _index++;
                _index = (byte)(_credits.Count % _index);
                _transition.State = TransitionState.TransitionOn;
            }

            _transition.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_credits.Count > 0)
            {
                Credit credit = _credits[_index];

            }
        }
    }
}