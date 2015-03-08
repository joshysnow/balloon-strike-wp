using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameFramework.Triggers;
using GameInterfaceFramework;
using GameInterfaceFramework.Actions;

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

        private WeaponManager _weaponManager;
        private BalloonManager _balloonManager;
        private PowerupManager _powerupManager;
        private ScoreAnimationManager _scoreManager;
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
#warning TODO: Need to retrieve previous state from storage.
            _gameState = GameState.Playing;

            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;

                _weaponManager = new WeaponManager();
                _balloonManager = new BalloonManager(graphics);
                _powerupManager = new PowerupManager(graphics);
                _sun = new Sun();

                // TODO: Need to call activate instead. Objects should decide to initialize or use old values.
                _weaponManager.Initialize();
                _balloonManager.Initialize();
                _powerupManager.Initialize();
                _sun.Initialize();

                _scoreManager = new ScoreAnimationManager();
                _scoreManager.Initialize();

                _balloonManager.Escaped += BalloonEscapedHandler;
                _balloonManager.Popped += BalloonPoppedHandler;
                _powerupManager.PickedUp += PowerupPickedUpHandler;
                _sun.Dead += SunDeadHandler;
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                // Load pause screen.
                View pauseView = new InputPopup("Paused", "Save and quit?", new LoadViewAction(2, this, new MainMenuView()));
                pauseView.ViewExiting += PauseViewExitingHandler;
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
            _sun.Update(gameTime);
        }

        private void UpdateGameOverState(GameTime gameTime)
        {
            if (IsExiting == false)
            {
                LoadView.Load(ViewManager, 1, new GameOverView());
            }
        }

        private void PauseViewExitingHandler(View view)
        {
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
