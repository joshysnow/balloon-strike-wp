using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

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

        private const string STORAGE_FILE_NAME = "PLAYER.xml";
                
        private int _currentScore;
        private bool _initialized;

        private Player()
        {
            _initialized = false;
        }

        public void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                // TODO: Load high score.
            }
        }

        public void Deactivate(bool saveScore)
        {
            // Save player data.
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("Player", 
                    new XAttribute("Score", 0),
                    new XAttribute("HighScore", HighScore)
                    );

                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Create))
                {
                    doc.Save(stream);
                }
            }
        }

        public void ResetScore()
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
