namespace GameFramework
{
    public class Player
    {
        public int HighScore
        {
            get;
            set;
        }

        public int CurrentScore
        {
            get;
            set;
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

        private static Player _instance;

        private Player()
        {

        }
    }
}
