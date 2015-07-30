using Microsoft.Xna.Framework;
using GameCore.Triggers;

namespace GameFramework.Triggers
{
    public class ScoreTrigger : Trigger
    {
        public int TriggerScore
        {
            get { return _scoreToTrigger; }
        }

        private int _scoreToTrigger;

        public ScoreTrigger(int scoreToTriggerAt)
        {
            _scoreToTrigger = scoreToTriggerAt;
        }

        protected override bool CanTrigger(GameTime gameTime)
        {
            return Player.Instance.CurrentScore >= _scoreToTrigger;
        }
    }
}
