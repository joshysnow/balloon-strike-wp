using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
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

        private WeaponManager _weaponManager;
        private BalloonManager _balloonManager;
        private PowerupManager _powerupManager;
        private ScoreManager _scoreManager;
        private GameState _gameState;
        private byte _sunLives;

        public GameView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
#warning TODO: Need to retrieve previous state from storage.
            _gameState = GameState.Playing;
            _sunLives = 3;

            if (!instancePreserved)
            {
                _weaponManager = new WeaponManager();
                _balloonManager = new BalloonManager();
                _powerupManager = new PowerupManager();
                _scoreManager = new ScoreManager();

                _balloonManager.Escaped += BalloonEscapedHandler;
                _balloonManager.Popped += BalloonPoppedHandler;
                _powerupManager.PickedUp += PowerupPickedUpHandler;
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
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
            spriteBatch.End();
        }

        private void UpdatePlayingState(GameTime gameTime)
        {
            _weaponManager.Update(gameTime);
            _balloonManager.Update(gameTime);
            _powerupManager.Update(gameTime);
            _scoreManager.Update(gameTime);
        }

        private void UpdateGameOverState(GameTime gameTime)
        {
            if (IsExiting == false)
            {
                Exit();
                LoadView.Load(ViewManager, 1, new GameOverView());
            }
        }

        private void BalloonEscapedHandler(Balloon balloon)
        {
            _sunLives--;

            if (_sunLives <= 0)
            {
                _gameState = GameState.GameOver;
            }
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
                case PowerupType.Missile:
                    // Pass to weapon manager.
                    _weaponManager.ApplyPowerup(powerup.Type);
                    break;
                default:
                    break;
            }
        }
    }
}
