using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public enum SunMood : byte
    {
        SuperHappy  = 0x01,
        Happy       = 0x02,
        Ok          = 0x04,
        Sad         = 0x08,
        Crying      = 0x0F
    }

    public delegate void SunDeadHandler();

    public class Sun : Serializable
    {
        public event SunDeadHandler Dead;

        public SunMood Mood
        {
            get;
            private set;
        }

        public bool Alive
        {
            get;
            private set;
        }

        public float MoodPosition
        {
            get
            {
                return ((float)_currentLives) / _totalLives;
            }
        }

        private static TimeSpan DURATION_SUPER_HAPPY    = TimeSpan.FromSeconds(2);
        private static TimeSpan DURATION_HAPPY          = TimeSpan.FromSeconds(0.5f);
        private static TimeSpan DURATION_OK             = TimeSpan.FromSeconds(0.5f);
        private static TimeSpan DURATION_SAD            = TimeSpan.FromSeconds(0.5f);
        private static TimeSpan DURATION_CRYING         = TimeSpan.FromSeconds(0.25f);

        private static TimeSpan FREQUENCY_SUPER_HAPPY   = TimeSpan.FromSeconds(0);
        private static TimeSpan FREQUENCY_HAPPY         = TimeSpan.FromSeconds(1);
        private static TimeSpan FREQUENCY_OK            = TimeSpan.FromSeconds(3);
        private static TimeSpan FREQUENCY_SAD           = TimeSpan.FromSeconds(0.5f);
        private static TimeSpan FREQUENCY_CRYING        = TimeSpan.FromSeconds(0.25f);

        private const float THRESHOLD_CRYING        = 0.1f;
        private const float THRESHOLD_SAD           = 0.4f;
        private const float THRESHOLD_HAPPY         = 0.7f;
        private const float THRESHOLD_SUPER_HAPPY   = 1f;

        private const string STORAGE_FILE_NAME = "SUN.xml";

        private AnimationPlayer _animationPlayer;
        private Animation _superHappyAnimation;
        private Animation _happyAnimation;
        private Animation _okAnimation;
        private Animation _sadAnimation;
        private Animation _cryingAnimation;
        private Animation _currentAnimation;
        private Pulse _pulse;
        private byte _totalLives;
        private byte _currentLives;

        public Sun()
        {
            _totalLives = 10;

            _pulse = new Pulse();
            _animationPlayer = new AnimationPlayer();
            _animationPlayer.SetPosition(new Vector2(10, 10));
        }

        public void Initialize()
        {
            Alive = true;
            Mood = SunMood.SuperHappy;
            _currentLives = 10;

            LoadResources();
            UpdateMood();
        }

        private void LoadResources()
        {
            ResourceManager resources = ResourceManager.Resources;
            _superHappyAnimation = resources.GetAnimation("sun_superhappy");
            _happyAnimation = resources.GetAnimation("sun_happy");
            _okAnimation = resources.GetAnimation("sun_ok");
            _sadAnimation = resources.GetAnimation("sun_sad");
            _cryingAnimation = resources.GetAnimation("sun_crying");
        }

        public void Activate(bool instancePreserved)
        {
            if (instancePreserved)
            {
                Initialize();
            }
            else
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(STORAGE_FILE_NAME))
                    {
                        LoadResources();

                        using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                        {
                            XDocument doc = XDocument.Load(stream);

                            XElement root = doc.Root;
                            _currentLives = byte.Parse(root.Attribute("Lives").Value);
                        }

                        UpdateMood();
                    }
                    else
                    {
                        Initialize();
                    }
                }
            }
        }

        public void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("Sun");
                root.Add(new XAttribute("Lives", _currentLives));
                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
        }

        public void LoseALife()
        {
            _currentLives--;

            if (_currentLives < 0)
                _currentLives = 0;

            UpdateMood();
        }

        public void Update(GameTime gameTime)
        {
            if (Mood == SunMood.Crying && _currentLives == 0)
            {
                RaiseDead();
            }

            _pulse.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentAnimation.Scale = MathHelper.Lerp(1, 1.25f, _pulse.Position);
            _animationPlayer.Draw(spriteBatch);
        }

        private void UpdateMood()
        {
            float moodPercentage = MoodPosition;

            if (moodPercentage <= THRESHOLD_CRYING)
            {
                Mood = SunMood.Crying;
                _currentAnimation = _cryingAnimation;
                _pulse.ChangeRhythm(DURATION_CRYING, FREQUENCY_CRYING);
            }
            else if (moodPercentage <= THRESHOLD_SAD)
            {
                Mood = SunMood.Sad;
                _currentAnimation = _sadAnimation;
                _pulse.ChangeRhythm(DURATION_SAD, FREQUENCY_SAD);
            }
            else if((moodPercentage > THRESHOLD_SAD) && (moodPercentage < THRESHOLD_HAPPY))
            {
                Mood = SunMood.Ok;
                _currentAnimation = _okAnimation;
                _pulse.ChangeRhythm(DURATION_OK, FREQUENCY_OK);
            }
            else if ((moodPercentage >= THRESHOLD_HAPPY) && (moodPercentage < THRESHOLD_SUPER_HAPPY))
            {
                Mood = SunMood.Happy;
                _currentAnimation = _happyAnimation;
                _pulse.ChangeRhythm(DURATION_HAPPY, FREQUENCY_HAPPY);
            }
            else
            {
                Mood = SunMood.SuperHappy;
                _currentAnimation = _superHappyAnimation;
                _pulse.ChangeRhythm(DURATION_SUPER_HAPPY, FREQUENCY_SUPER_HAPPY);
            }

            _animationPlayer.SetAnimation(_currentAnimation);
        }

        private void RaiseDead()
        {
            if (Dead != null)
                Dead();
        }
    }
}
