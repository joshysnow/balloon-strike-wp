using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public class ScoreDisplay
    {
        private const string STORAGE_FILE_NAME = "SCORE_DISPLAY.xml";

        private SpriteFont[] _fonts;
        private TimeSpan _transitionTime;
        private float _position;
        private int _direction;
        private bool _transition;

        public ScoreDisplay()
        {
            _transitionTime = TimeSpan.FromSeconds(0.33f); // 330ms
            _position = 0;
            _direction = 1;
            _transition = false;
        }

        public void Activate(bool instancePreserved)
        {
            if (instancePreserved)
            {
                // Nothing to do here, instance is preserved after all.
            }
            else
            {
                // Load resources and subscribe to events.
                Initialize();

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(STORAGE_FILE_NAME))
                    {
                        using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                        {
                            XDocument doc = XDocument.Load(stream);
                            XElement root = doc.Root;

                            _position = float.Parse(root.Attribute("Position").Value);
                            _direction = int.Parse(root.Attribute("Direction").Value);
                            _transition = bool.Parse(root.Attribute("Transition").Value);
                        }

                        // Delete the hydration file to avoid confusion.
                        storage.DeleteFile(STORAGE_FILE_NAME);
                    }
                }
            }
        }

        public void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {                
                XElement root = new XElement(
                    "ScoreDisplay",
                    new XAttribute("Position", _position),
                    new XAttribute("Direction", _direction),
                    new XAttribute("Transition", _transition)
                    );

                XDocument doc = new XDocument();
                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_transition)
            {
                // How much to move by. Time for transition halved as the animation is played twice (once backwards).
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / (_transitionTime.TotalMilliseconds / 2));

                // Increase or decrease position.
                _position += delta * _direction;
                _position = MathHelper.Clamp(_position, 0, 1);

                // Achieved 50% of the overall transition, time to decrease the size to complete the animation!
                if ((_direction == 1) && (_position == 1))
                {
                    _direction = -1;
                }

                // If the animation is complete (decreasing and finished at normal size), stop animating!
                if ((_direction == -1) && (_position <= (1 / 8192)))
                {
                    _position = 0;
                    _direction = 1;
                    _transition = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int score = Player.Instance.CurrentScore;
            SpriteFont font = _fonts[(int)(_position * 9)];

            Vector2 scoreLength = font.MeasureString(score.ToString());
            Vector2 position = new Vector2(480 - scoreLength.X - 10, 0); // Screen width - horizontal length of text - spacing from the side, top margin.

            spriteBatch.DrawString(font, score.ToString(), position, Color.Yellow);
        }

        private void Initialize()
        {
            LoadFonts();

            // Be notified when the players score is increased.
            Player.Instance.ScoreUpdated += PlayerScoreUpdatedHandler;
        }

        private void LoadFonts()
        {
            _fonts = new SpriteFont[10];

            ResourceManager manager = ResourceManager.Resources;

            for (int i = 0; i < _fonts.Length; i++)
            {
                _fonts[i] = manager.GetFont("font_score" + i);
            }
        }

        private void ResetAnimation()
        {
            // Reset animation.
            _position = 0;
            _direction = 1;
            _transition = true;
        }

        private void PlayerScoreUpdatedHandler(int newScore)
        {
            ResetAnimation();
        }
    }
}
