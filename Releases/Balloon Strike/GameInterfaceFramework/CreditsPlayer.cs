using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameInterfaceFramework
{
    public class CreditsPlayer
    {
        private List<Credit> _credits;
        private Transition _transition;
        private byte _index;
        private bool _stopping;

        public CreditsPlayer()
        {
            _credits = new List<Credit>();
            _transition = new Transition()
            {
                TransitionOn = TimeSpan.FromSeconds(1.5),
                TransitionOff = TimeSpan.FromSeconds(1.5),
                Active = TimeSpan.FromSeconds(1)
            };
            _index = 0;
            _stopping = false;
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

        public void End(TimeSpan transitionOffTime)
        {
            _transition.TransitionOff = transitionOffTime;
            _transition.State = TransitionState.TransitionOff;
            _stopping = true;
        }

        public void Update(GameTime gameTime)
        {
            if ((_stopping == false) && _transition.State == TransitionState.Hidden)
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