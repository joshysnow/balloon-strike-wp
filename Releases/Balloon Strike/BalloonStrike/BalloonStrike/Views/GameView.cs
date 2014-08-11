using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore.Triggers;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class GameView : View
    {
        private enum GameState : byte
        {
            Playing     = 0x01,
            GameOver    = 0x02
        }

        private TriggerManager _triggerManager;
        private WeaponManager _weaponManager;
        private BalloonManager _balloonManager;
        private PowerupManager _powerupManager;
        private ScoreManager _scoreManager;
        private Sun _sun;
        private GameState _gameState;

        public GameView()
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);

            EnabledGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
#warning TODO: Need to retrieve previous state from storage.
            _gameState = GameState.Playing;

            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;

                _triggerManager = new TriggerManager();
                _weaponManager = new WeaponManager();
                _balloonManager = new BalloonManager(graphics, _triggerManager);
                _powerupManager = new PowerupManager(graphics, _triggerManager);
                _scoreManager = new ScoreManager();
                _sun = new Sun();
                _sun.Initialize();

                _balloonManager.Escaped += BalloonEscapedHandler;
                _balloonManager.Popped += BalloonPoppedHandler;
                _powerupManager.PickedUp += PowerupPickedUpHandler;
                _sun.Dead += SunDeadHandler;

                Player player = Player.Instance;
                player.CurrentScore = 0;
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                // Load pause screen.
                ViewManager.AddView(new InputPopup("Quit?"));
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
            _scoreManager.Draw(spriteBatch);
            _sun.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void UpdatePlayingState(GameTime gameTime)
        {
            _weaponManager.Update(gameTime);
            _balloonManager.Update(gameTime);
            _powerupManager.Update(gameTime);
            _scoreManager.Update(gameTime);
            _triggerManager.Update(gameTime, _scoreManager.Score);
            _sun.Update(gameTime);
        }

        private void UpdateGameOverState(GameTime gameTime)
        {
            if (IsExiting == false)
            {
                Player player = Player.Instance;
                player.CurrentScore = _scoreManager.Score;

                LoadView.Load(ViewManager, 1, new GameOverView());
            }
        }

        private void BalloonEscapedHandler(Balloon balloon)
        {
            _sun.LoseALife();
        }

        private void BalloonPoppedHandler(Balloon balloon)
        {
            _scoreManager.IncreaseScore(1);
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
