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

namespace sandbox5
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
        private int _spawnCount;
        private GameState _gameState;
        private Random _randomPosition;

        private LinkedList<Balloon> _balloonMemory;
        private List<Balloon> _balloons;
        private float _balloonScale;

        private Vector2 _redVelocity;
        private Vector2 _greenVelocity;
        private Vector2 _blueVelocity;

        private SoundEffect _popEffect;
        private Animation _popAnimation;
        private Animation _redMoveAnimation;
        private Animation _greenMoveAnimation;
        private Animation _blueMoveAnimation;

        private MonoSpawnTimer _greenTimer;
        private VariableSpawnTimer _blueTimer;
        private VariableSpawnTimer _redTimer;

        private List<GestureSample> _gestures;

        private SpriteFont _debugFont;
        private SpriteFont _displayFont;
        private SpriteFont _gameOverFont;
        private SpriteFont _gameOverScoreFont;
        private Vector2 _debugPosition;
        private Vector2 _sunPosition;
        private Vector2 _gameOverPosition;
        private const byte _spacing = 10;

        private int _screenWidth;
        private int _screenHeight;

        private const string _gameOverText = "GAME OVER";

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

            this.Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            this.TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            this.InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _greenTimer = new MonoSpawnTimer();
            _greenTimer.Initialize(1500);

            _blueTimer = new VariableSpawnTimer();
            _blueTimer.Initialize(5000, 0.98f, 1500);

            _redTimer = new VariableSpawnTimer();
            _redTimer.Initialize(15000, 0.98f, 2500);

            _spawnCount = 0;
            _randomPosition = new Random(DateTime.Now.Millisecond);

            _redVelocity = new Vector2(0, -9.2f);
            _greenVelocity = new Vector2(0, -5.1f);
            _blueVelocity = new Vector2(0, -7.15f);

            _balloonMemory = new LinkedList<Balloon>();
            _balloons = new List<Balloon>();
            _balloonScale = 0.5f;

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

            Texture2D redTexture = this.Content.Load<Texture2D>("Balloons/red200");
            Texture2D blueTexture = this.Content.Load<Texture2D>("Balloons/blue200");
            Texture2D greenTexture = this.Content.Load<Texture2D>("Balloons/green200");
            Texture2D popTexture = this.Content.Load<Texture2D>("Effects/explosion");
            _debugFont = this.Content.Load<SpriteFont>("DebugText");
            _displayFont = this.Content.Load<SpriteFont>("DisplayText");
            _gameOverFont = this.Content.Load<SpriteFont>("GameOverText");
            _gameOverScoreFont = this.Content.Load<SpriteFont>("ScoreText");
            _popEffect = this.Content.Load<SoundEffect>("Sounds/snowball_car_impact1");

            _popAnimation = new Animation(popTexture, false, popTexture.Width, popTexture.Height, 125f, 0.25f);
            _redMoveAnimation = new Animation(redTexture, true, redTexture.Width, redTexture.Height, 0f, 0.5f);
            _greenMoveAnimation = new Animation(greenTexture, true, greenTexture.Width, greenTexture.Height, 0f, 0.5f);
            _blueMoveAnimation = new Animation(blueTexture, true, blueTexture.Width, blueTexture.Height, 0f, 0.5f);

            _sunPosition = new Vector2(_spacing, 0);
            _debugPosition = new Vector2(_spacing, _screenHeight - _debugFont.LineSpacing);
            Vector2 gameOverTextSize = _gameOverFont.MeasureString(_gameOverText);
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
            this.UpdatePlayerInput();
            this.UpdateBalloons(gameTime);
            this.UpdateSpawnTimers(gameTime);
        }

        private void UpdateGameOverState(GameTime gameTime)
        {
            if (TouchPanel.IsGestureAvailable)
            {
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
                foreach (GestureSample gesture in _gestures)
                {
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        List<Balloon> intersections = (from b in _balloons where b.Intersects(gesture.Position) select b).ToList();
                        if (intersections.Count() > 0)
                        {
                            intersections.Last().Pop();
                        }
                    }
                }
            }

            _gestures.Clear();
        }

        private void UpdateBalloons(GameTime gameTime)
        {
            int index = 0;
            Balloon balloon;
            while (index < _balloons.Count)
            {
                balloon = _balloons[index];

                switch (balloon.State)
                {
                    case BalloonState.Alive:
                    case BalloonState.Dying:
                        {
                            balloon.Update(gameTime);
                            index++;
                        }
                        break;
                    case BalloonState.Popped:
                        {
                            balloon.Update(gameTime);
                            _score += 10;
                            index++;
                        }
                        break;
                    case BalloonState.Escaped:
                        {
                            balloon.Update(gameTime);
                            _sunsMood--;
                            index++;
                        }
                        break;
                    case BalloonState.Dead:
                        {
                            balloon.Uninitialize();
                            _balloonMemory.AddFirst(balloon);
                            _balloons.RemoveAt(index);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (_sunsMood <= 0)
            {
                _gameState = GameState.GameOver;
                _balloons.Clear();
            }
        }

        private void UpdateSpawnTimers(GameTime gameTime)
        {
            if (_greenTimer.Update(gameTime))
            {
                SpawnBalloon(BalloonColour.Green);
            }

            if (_blueTimer.Update(gameTime))
            {
                SpawnBalloon(BalloonColour.Blue);
            }

            if (_redTimer.Update(gameTime))
            {
                SpawnBalloon(BalloonColour.Red);
            }
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
            int index = 0;
            while (index < _balloons.Count)
            {
                _balloons[index].Draw(_spriteBatch);
                index++;
            }

            string scoreText = "Score: " + _score;
            Vector2 scoreTextLength = _displayFont.MeasureString(scoreText);
            Vector2 scorePosition = new Vector2(_screenWidth - scoreTextLength.X - _spacing, 0);

            _spriteBatch.DrawString(_displayFont, "Sun Mood: " + _sunsMood, _sunPosition, Color.Gold);
            _spriteBatch.DrawString(_displayFont, scoreText, scorePosition, Color.Yellow);
            _spriteBatch.DrawString(_debugFont, "DEBUG - Pool : " + _balloonMemory.Count + " - Spawned : " + _spawnCount, _debugPosition, Color.White);
        }

        private void DrawGameOverState()
        {
            _spriteBatch.DrawString(_gameOverFont, _gameOverText, _gameOverPosition, Color.Crimson);
            string text = "YOUR SCORE: " + _score;
            Vector2 textSize = _gameOverScoreFont.MeasureString(text);
            Vector2 scorePosition =  new Vector2((_screenWidth / 2) - (textSize.X / 2), (_gameOverPosition.Y + _gameOverScoreFont.LineSpacing));
            _spriteBatch.DrawString(_gameOverScoreFont, text, scorePosition, Color.Crimson);
        }

        private void SpawnBalloon(BalloonColour colour)
        {
            Balloon spawn;

            if (_balloonMemory.Count > 0)
            {
                spawn = _balloonMemory.First.Value;
                _balloonMemory.RemoveFirst();
            }
            else
            {
                spawn = new Balloon();
            }

            _spawnCount++;
            int x = _randomPosition.Next(_screenWidth - (int)(_redMoveAnimation.AnimationTexture.Width * _balloonScale));
            spawn.Colour = colour;

            Vector2 velocity = new Vector2(0, 3.1f);
            Animation moveAnimation = _greenMoveAnimation;

            switch (colour)
            {
                case BalloonColour.Red:
                    velocity = _redVelocity;
                    moveAnimation = _redMoveAnimation;
                    break;
                case BalloonColour.Green:
                    velocity = _greenVelocity;
                    break;
                case BalloonColour.Blue:
                    moveAnimation = _blueMoveAnimation;
                    velocity = _blueVelocity;
                    break;
            }

            spawn.Initialize(moveAnimation, _popAnimation, _popEffect, new Vector2(x, _screenHeight), velocity, _balloonScale);
            _balloons.Add(spawn);
        }

        private void Setup()
        {
            // Populate pool.
            Balloon b;
            while (_balloonMemory.Count < 10)
            {
                b = new Balloon();
                _balloonMemory.AddFirst(b);
            }

            _score = 0;
            _sunsMood = 3;
            _gameState = GameState.Playing;
        }

        private void SandboxDeactivated(object sender, EventArgs e)
        {
            this.Exit();
        }
    }
}
