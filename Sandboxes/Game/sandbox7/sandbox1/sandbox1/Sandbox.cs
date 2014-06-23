using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace sandbox7
{
    public enum GameState : byte
    {
        Playing     = 0x01,
        GameOver    = 0x02
    }

    /// <summary>
    /// This is the sandbox environment generator to test and create new things!
    /// </summary>
    public class Sandbox : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _score;
        private int _sunsMood;
        private GameState _gameState;

        private Player _player;
        private BalloonManager _balloonManager;
        private PowerupManager _powerupManager;

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

        public Sandbox()
        {
            _screenHeight = 800;
            _screenWidth = 480;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.ToggleFullScreen();
            _graphics.SupportedOrientations = DisplayOrientation.Portrait;

            Guide.IsScreenSaverEnabled = false;
            base.IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
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

            _player = new Player();
            _balloonManager = new BalloonManager();
            _powerupManager = new PowerupManager();

            _balloonManager.Popped += _balloonManager_Popped;
            _balloonManager.Escaped += _balloonManager_Escaped;
            _powerupManager.PickedUp += _powerupManager_PickedUp;

            TouchPanel.EnabledGestures = GestureType.Tap;
            _gestures = new List<GestureSample>();

            // When the game is not active, close.
            this.Deactivated += SandboxDeactivated;

            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _debugFont = this.Content.Load<SpriteFont>("DebugText");
            _displayFont = this.Content.Load<SpriteFont>("DisplayText");
            _gameOverFont = this.Content.Load<SpriteFont>("GameOverText");
            _gameOverScoreFont = this.Content.Load<SpriteFont>("ScoreText");

            _sunPosition = new Vector2(_spacing, 0);
            _debugPosition = new Vector2(_spacing, _screenHeight - _debugFont.LineSpacing);
            Vector2 gameOverTextSize = _gameOverFont.MeasureString(GAME_OVER_TEXT);
            _gameOverPosition = new Vector2((_screenWidth / 2) - (gameOverTextSize.X / 2), (_screenHeight / 2) - (gameOverTextSize.Y / 2));

            this.Setup();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            switch (_gameState)
            {
                case GameState.Playing:
                    {
                        this.UpdatePlayingState(gameTime);
                    }
                    break;
                case GameState.GameOver:
                    {
                        this.UpdateGameOverState(gameTime);
                    }
                    break;
                default:
                    break;
            }
            
            base.Update(gameTime);
        }

        private void UpdatePlayingState(GameTime gameTime)
        {
            UpdatePlayerInput();
            _player.Update(gameTime);
            _balloonManager.Update(gameTime);
            _powerupManager.Update(gameTime);

            if (_sunsMood <= 0)
            {
                this.Reset();
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

                this.Setup();
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
                _player.UpdateInput(gestureArray.Last().Position);
                _powerupManager.UpdatePlayerInput(gestureArray);
                _balloonManager.UpdatePlayerInput(gestureArray);
            }

            _gestures.Clear();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_gameState)
            {
                case GameState.Playing:
                    {
                        this.DrawPlayingState();
                    }
                    break;
                case GameState.GameOver:
                    {
                        this.DrawGameOverState();
                    }
                    break;
                default:
                    break;
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawPlayingState()
        {
            _balloonManager.Draw(_spriteBatch);
            _powerupManager.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);

            string scoreText = "Score: " + _score;
            Vector2 scoreTextLength = _displayFont.MeasureString(scoreText);
            Vector2 scorePosition = new Vector2(_screenWidth - scoreTextLength.X - _spacing, 0);

            _spriteBatch.DrawString(_displayFont, "Sun Mood: " + _sunsMood, _sunPosition, Color.Gold);
            _spriteBatch.DrawString(_displayFont, scoreText, scorePosition, Color.Yellow);
            //_spriteBatch.DrawString(_debugFont, "DEBUG - Pool : " + _balloonMemory.Count + " - Spawned : " + _spawnCount, _debugPosition, Color.White);
        }

        private void DrawGameOverState()
        {
            _spriteBatch.DrawString(_gameOverFont, GAME_OVER_TEXT, _gameOverPosition, Color.Crimson);
            string text = "YOUR SCORE: " + _score;
            Vector2 textSize = _gameOverScoreFont.MeasureString(text);
            Vector2 scorePosition =  new Vector2((_screenWidth / 2) - (textSize.X / 2), (_gameOverPosition.Y + _gameOverScoreFont.LineSpacing));
            _spriteBatch.DrawString(_gameOverScoreFont, text, scorePosition, Color.Crimson);
        }

        private void Setup()
        {
            _score = 0;
            _sunsMood = 3;
            _gameState = GameState.Playing;
        }

        private void Reset()
        {
            _gameState = GameState.GameOver;
            _player.ChangeWeapon(WeaponType.Finger);
            _balloonManager.Reset();
            _powerupManager.Reset();
        }

        private void SandboxDeactivated(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void _balloonManager_Popped(Balloon balloon)
        {
            _score++;
        }

        private void _balloonManager_Escaped(Balloon balloon)
        {
            _sunsMood--;
        }

        private void _powerupManager_PickedUp(Powerup powerup)
        {
            // TODO: Change the weapon or affect the balloons in some way.
        }
    }
}
