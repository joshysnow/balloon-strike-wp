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
            get { return _highScore; }
            set { _highScore = value; }
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

        private int _highScore;
        private int _currentScore;

        private Player() { }

        public void Activate(bool instancePreserved, bool newGame)
        {
            if (!instancePreserved)
            {
                // Load high score.
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(STORAGE_FILE_NAME) && !newGame)
                    {
                        using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                        {
                            XDocument doc = XDocument.Load(stream);
                            XElement xPlayer = doc.Root;

                            _highScore = int.Parse(xPlayer.Attribute("HighScore").Value);

                            // Only if this is a new game do we not rehydrate the stored score.
                            if (!newGame)
                                _currentScore = int.Parse(xPlayer.Attribute("Score").Value);
                        }

                        // Note: The file isn't deleted in case something goes wrong we still have
                        // the players high score stored on disk.
                    }
                }
            }

            if (newGame)
                _currentScore = 0;
        }

        public void Deactivate()
        {
            // Save player data.
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Delete the file so it is ready to be replaced.
                if (storage.FileExists(STORAGE_FILE_NAME))
                    storage.DeleteFile(STORAGE_FILE_NAME);

                XDocument doc = new XDocument();
                XElement root = new XElement("Player", 
                    new XAttribute("Score", _currentScore),
                    new XAttribute("HighScore", _highScore)
                    );

                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
        }

        private void RaiseScoreUpdated()
        {
            if (ScoreUpdated != null)
                ScoreUpdated(_currentScore);
        }
    }
}
