using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameFramework.Triggers;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class GameView : View
    {
        private enum GameState : byte
        {
            Playing     = 0x01,
            GameOver    = 0x02,
            Paused      = 0x04
        }

        private const string STORAGE_FILE_NAME = "GAME_VIEW.xml";

        private WeaponManager _weaponManager;
        private BalloonManager _balloonManager;
        private PowerupManager _powerupManager;
        private ScoreDisplay _scoreDisplay;
        private Sun _sun;
        private GameState _gameState;

        public GameView()
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);

            ViewGestures = GestureType.Tap;

            IsSerializable = true;
        }

        public override void Activate(bool instancePreserved)
        {
            GraphicsDevice graphics = ViewManager.GraphicsDevice;

            Player.Instance.CurrentScore = 0;

            _sun = new Sun();
            _sun.Activate(instancePreserved);

            _scoreDisplay = new ScoreDisplay();
            _scoreDisplay.Activate(instancePreserved);

            _weaponManager = new WeaponManager();
            _weaponManager.Activate(instancePreserved);

            _balloonManager = new BalloonManager(graphics);
            _balloonManager.Activate(instancePreserved);

            _powerupManager = new PowerupManager(graphics);
            _powerupManager.Activate(instancePreserved);

            // Rehyrate the game view
            if (!instancePreserved)
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(STORAGE_FILE_NAME))
                    {
                        using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                        {
                            XDocument doc = XDocument.Load(stream);
                            XElement root = doc.Root;

                            _gameState = (GameState)Enum.Parse(_gameState.GetType(), root.Attribute("State").Value, false);
                        }

                        storage.DeleteFile(STORAGE_FILE_NAME);
                    }
                    else
                    {
                        // Must've failed to dehydrate the game, setup as new
                        _gameState = GameState.Playing;
                    }
                }

                _balloonManager.Escaped += BalloonEscapedHandler;
                _balloonManager.Popped += BalloonPoppedHandler;
                _powerupManager.PickedUp += PowerupPickedUpHandler;
                _sun.Dead += SunDeadHandler;
            }
        }

        public override void Deactivate()
        {
            _sun.Deactivate();
            _scoreDisplay.Deactivate();
            _weaponManager.Deactivate();
            _balloonManager.Deactivate();
            _powerupManager.Deactivate();

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XElement root = new XElement("View");
                root.Add(new XAttribute("State", _gameState));

                XDocument doc = new XDocument();
                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                // Load pause screen.
                InputPopup pauseView = new InputPopup("Paused", "Quit?");
                pauseView.Result += PauseViewResultHandler;
                ViewManager.AddView(pauseView);

                _gameState = GameState.Paused;
            }
            else
            {
                if (controls.Gestures.Length > 0)
                {
                    GestureSample[] gestureArray = controls.Gestures;
                    Weapon currentWeapon = _weaponManager.CurrentWeapon;
                    GestureSample[] remainingGestures;

                    _powerupManager.UpdatePlayerInput(gestureArray, currentWeapon, out remainingGestures);
                    _weaponManager.UpdateInput(remainingGestures);
                    _balloonManager.UpdatePlayerInput(remainingGestures, currentWeapon, out remainingGestures);
                }
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (!covered)
            {
                switch (_gameState)
                {
                    case GameState.Playing:
                        UpdatePlayingState(gameTime);
                        break;
                    case GameState.GameOver:
                        UpdateGameOverState(gameTime);
                        break;
                    case GameState.Paused:
                    default:
                        break;
                }
            }

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;

            spriteBatch.Begin();
            _balloonManager.Draw(spriteBatch);
            _powerupManager.Draw(spriteBatch);
            _weaponManager.Draw(spriteBatch);
            _scoreDisplay.Draw(spriteBatch);
            _sun.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void UpdatePlayingState(GameTime gameTime)
        {
            _weaponManager.Update(gameTime);
            _balloonManager.Update(gameTime);
            _powerupManager.Update(gameTime);
            _scoreDisplay.Update(gameTime);
            _sun.Update(gameTime);
        }

        private void UpdateGameOverState(GameTime gameTime)
        {
            if (IsExiting == false)
            {
                LoadView.Load(ViewManager, 1, new GameOverView());
            }
        }

        private void PauseViewResultHandler(ResultState result)
        {
            if (result == ResultState.YES)
                LoadView.Load(ViewManager, 2, new MainMenuView());
            else
                _gameState = GameState.Playing;
        }

        private void BalloonEscapedHandler(Balloon balloon)
        {
            _sun.LoseALife();
        }

        private void BalloonPoppedHandler(Balloon balloon)
        {
            Player.Instance.CurrentScore += 1;
        }

        private void PowerupPickedUpHandler(Powerup powerup)
        {
            switch (powerup.Type)
            {
                case PowerupType.Freeze:
                case PowerupType.Nuke:
                    // Pass to balloon manager.
                    _balloonManager.ApplyPowerup(powerup.Type);
                    break;
                case PowerupType.Shell:
                case PowerupType.Rocket:
                    // Pass to weapon manager.
                    _weaponManager.ApplyPowerup(powerup.Type);
                    break;
                default:
                    break;
            }
        }

        private void SunDeadHandler()
        {
            _gameState = GameState.GameOver;
        }
    }
}
