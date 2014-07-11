using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using GameFramework;
using GameInterfaceFramework;
using Microsoft.Phone.Shell;
using BalloonStrike.Views;

namespace BalloonStrike
{
    public enum GameState : byte
    {
        Playing = 0x01,
        GameOver = 0x02
    }

    /// <summary>
    /// This is the sandbox environment generator to test and create new things!
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _sunsMood;
        private GameState _gameState;

        private WeaponManager _weaponManager;
        private BalloonManager _balloonManager;
        private PowerupManager _powerupManager;
        private ScoreManager _scoreManager;

        private List<GestureSample> _gestures;

        private SpriteFont _debugFont;
        private SpriteFont _displayFont;
        private SpriteFont _gameOverFont;
        private SpriteFont _gameOverScoreFont;
        private Vector2 _debugPosition;
        private Vector2 _sunPosition;
        private Vector2 _gameOverPosition;

        private short _screenWidth;
        private short _screenHeight;

        private const string GAME_OVER_TEXT = "GAME OVER";
        private const byte _spacing = 10;

        private ViewManager _viewManager;

        public Game()
        {
            Content.RootDirectory = "Content";
            _screenHeight = 800;
            _screenWidth = 480;

            InitializeGraphics();
            InitializePhoneServices();
            InitializeGameServices();
        }

        private void InitializeGraphics()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.PreferredBackBufferWidth = 480;
            _graphics.IsFullScreen = true;
            _graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        private void InitializePhoneServices()
        {
            PhoneApplicationService.Current.Activated += GameActivated;
            PhoneApplicationService.Current.Deactivated += GameDeactivated;
            PhoneApplicationService.Current.Launching += GameLaunching;
        }

        private void InitializeGameServices()
        {
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            Guide.IsScreenSaverEnabled = true;
            IsFixedTimeStep = false;

            _viewManager = new ViewManager(this);
            Components.Add(_viewManager);
        }

        private void InitializeGame()
        {
            _viewManager.AddView(new SplashView());
        }

        private void GameActivated(object sender, ActivatedEventArgs e)
        {
            if (_viewManager.Activate(e.IsApplicationInstancePreserved))
            {
                InitializeGame();
            }
        }

        private void GameDeactivated(object sender, DeactivatedEventArgs e)
        {
            _viewManager.Deactivate();
        }

        private void GameLaunching(object sender, LaunchingEventArgs e)
        {
            InitializeGame();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ResourceManager.Initialize(Content);
            //_weaponManager = new WeaponManager();
            //_balloonManager = new BalloonManager();
            //_powerupManager = new PowerupManager();
            //_scoreManager = new ScoreManager();

            //_balloonManager.Popped += BalloonPoppedHandler;
            //_balloonManager.Escaped += BalloonEscapedHandler;
            //_powerupManager.PickedUp += PowerupPickedUpHandler;

            //TouchPanel.EnabledGestures = GestureType.Tap;
            //_gestures = new List<GestureSample>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //// Create a new SpriteBatch, which can be used to draw textures.
            //_spriteBatch = new SpriteBatch(GraphicsDevice);

            //_debugFont = this.Content.Load<SpriteFont>("DebugText");
            //_displayFont = this.Content.Load<SpriteFont>("DisplayText");
            //_gameOverFont = this.Content.Load<SpriteFont>("GameOverText");
            //_gameOverScoreFont = this.Content.Load<SpriteFont>("ScoreText");

            //_sunPosition = new Vector2(_spacing, 0);
            //_debugPosition = new Vector2(_spacing, _screenHeight - _debugFont.LineSpacing);
            //Vector2 gameOverTextSize = _gameOverFont.MeasureString(GAME_OVER_TEXT);
            //_gameOverPosition = new Vector2((_screenWidth / 2) - (gameOverTextSize.X / 2), (_screenHeight / 2) - (gameOverTextSize.Y / 2));

            //ChangeToPlayingState();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //{
            //    this.Exit();
            //}

            //switch (_gameState)
            //{
            //    case GameState.Playing:
            //        {
            //            this.UpdatePlayingState(gameTime);
            //        }
            //        break;
            //    case GameState.GameOver:
            //        {
            //            this.UpdateGameOverState(gameTime);
            //        }
            //        break;
            //    default:
            //        break;
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //_spriteBatch.Begin();

            //switch (_gameState)
            //{
            //    case GameState.Playing:
            //        {
            //            this.DrawPlayingState();
            //        }
            //        break;
            //    case GameState.GameOver:
            //        {
            //            this.DrawGameOverState();
            //        }
            //        break;
            //    default:
            //        break;
            //}

            //_spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdatePlayingState(GameTime gameTime)
        {
            UpdatePlayerInput();
            _weaponManager.Update(gameTime);
            _balloonManager.Update(gameTime);
            _powerupManager.Update(gameTime);
            _scoreManager.Update(gameTime);

            if (_sunsMood <= 0)
            {
                ChangeToGameOverState();
            }
        }

        private void UpdateGameOverState(GameTime gameTime)
        {
            if (TouchPanel.IsGestureAvailable)
            {
                while (TouchPanel.IsGestureAvailable)
                {
                    TouchPanel.ReadGesture();
                }

                this.ChangeToPlayingState();
            }
        }

        private void UpdatePlayerInput()
        {
            while (TouchPanel.IsGestureAvailable)
            {
                _gestures.Add(TouchPanel.ReadGesture());
            }

            if (_gestures.Count > 0)
            {
                GestureSample[] gestureArray = _gestures.ToArray();
                Weapon currentWeapon = _weaponManager.CurrentWeapon;
                GestureSample[] remainingGestures;

                _powerupManager.UpdatePlayerInput(gestureArray, currentWeapon, out remainingGestures);
                _weaponManager.UpdateInput(remainingGestures);
                _balloonManager.UpdatePlayerInput(remainingGestures, currentWeapon, out remainingGestures);
            }

            _gestures.Clear();
        }

        private void DrawPlayingState()
        {
            _balloonManager.Draw(_spriteBatch);
            _powerupManager.Draw(_spriteBatch);
            _weaponManager.Draw(_spriteBatch);
            _scoreManager.Draw(_spriteBatch);

            _spriteBatch.DrawString(_displayFont, "Sun Mood: " + _sunsMood, _sunPosition, Color.Gold);
        }

        private void DrawGameOverState()
        {
            _spriteBatch.DrawString(_gameOverFont, GAME_OVER_TEXT, _gameOverPosition, Color.Crimson);
            string text = "YOUR SCORE: " + _scoreManager.Score;
            Vector2 textSize = _gameOverScoreFont.MeasureString(text);
            Vector2 scorePosition = new Vector2((_screenWidth / 2) - (textSize.X / 2), (_gameOverPosition.Y + _gameOverScoreFont.LineSpacing));
            _spriteBatch.DrawString(_gameOverScoreFont, text, scorePosition, Color.Crimson);
        }

        private void ChangeToPlayingState()
        {
            _sunsMood = 3;
            _gameState = GameState.Playing;

            _weaponManager.Reset();
            _balloonManager.Reset();
            _powerupManager.Reset();
            _scoreManager.Reset();
        }

        private void ChangeToGameOverState()
        {
            _gameState = GameState.GameOver;
        }

        private void BalloonPoppedHandler(Balloon balloon)
        {
            _scoreManager.IncreaseScore(1);
        }

        private void BalloonEscapedHandler(Balloon balloon)
        {
            _sunsMood--;
        }

        private void PowerupPickedUpHandler(Powerup powerup)
        {
#warning TODO: Change the weapon or affect the balloons in some way.
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
