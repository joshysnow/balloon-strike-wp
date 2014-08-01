namespace GameCore.Triggers
{
    public class ScoreTrigger : Trigger
    {
        public ScoreTrigger(int scoreToTriggerAt)
        {
            _triggerPoint = scoreToTriggerAt;
        }

        public void Update(int currentScore)
        {
            if (currentScore >= _triggerPoint)
            {
                RaiseTriggered();
            }
        }
    }
}
