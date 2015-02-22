namespace GameFramework
{
    public delegate void ScoreUpdated(int newScore);

    /// <summary>
    /// The player class holds attributes about the
    /// current players game session like the score.
    /// 
    /// It is imagined that this class would be used across
    /// the whole application to display the high score,
    /// store it and allow triggers to know what the
    /// current score is.
    /// </summary>
    public class Player
    {
        public int HighScore
        {
            get;
            set;
        }

        public int CurrentScore
        {
            get
            {
                return _currentScore;
            }
            set
            {
                _currentScore = value;
                RaiseScoreUpdated();
            }
        }

        public static Player Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }

                return _instance;
            }
        }

        public event ScoreUpdated ScoreUpdated;

        private static Player _instance;
        private int _currentScore;

        private Player()
        {
            _currentScore = 0;
            HighScore = 0;
        }

        public void Initialize()
        {
#warning TODO: Get high score from disk.
        }

        public void Reset()
        {
            _currentScore = 0;
        }

        private void RaiseScoreUpdated()
        {
            if (ScoreUpdated != null)
                ScoreUpdated(_currentScore);
        }
    }
}
