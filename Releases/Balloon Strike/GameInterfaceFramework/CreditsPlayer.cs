using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFramework;

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

            _transition.TransitionOn = TimeSpan.FromSeconds(1.5);
            _transition.TransitionOff = TimeSpan.FromSeconds(1.5);
        }

        public void AddCredit(Credit credit)
        {
            if (credit != null)
            {
                _credits.Add(credit);
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
                _index = (byte)(_index % _credits.Count);
                _transition.State = TransitionState.TransitionOn;
            }

            _transition.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_credits.Count > 0)
            {
                Credit credit = _credits[_index];
                spriteBatch.DrawString(credit.Model.TitleFont, credit.Title, credit.TitlePosition, Color.Black * _transition.TransitionAlpha);
                spriteBatch.DrawString(credit.Model.NameFont, credit.Name, credit.NamePosition, Color.Black * _transition.TransitionAlpha);         
            }
        }
    }
}