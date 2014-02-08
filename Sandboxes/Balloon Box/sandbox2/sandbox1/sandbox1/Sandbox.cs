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

namespace sandbox2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Sandbox : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _greenTexture;
        private Texture2D _redTexture;
        private Texture2D _blueTexture;

        private int _spawnCount;
        private Vector2 _spawnVelocity;
        private Random _randomPosition;

        private LinkedList<Balloon> _balloonMemory;
        private List<Balloon> _balloons;

        private SpriteFont _debugFont;

        private SpawnTimer _spawnTimer;

        private int _screenWidth;
        private int _screenHeight;

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
            _spawnTimer = new SpawnTimer();
            _spawnTimer.Initialize(5000);

            _spawnCount = 0;
            _spawnVelocity = new Vector2(0, -5.1f);
            _randomPosition = new Random(DateTime.Now.Millisecond);

            _balloonMemory = new LinkedList<Balloon>();
            _balloons = new List<Balloon>();

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

            _redTexture = this.Content.Load<Texture2D>("Balloons/red200");
            _blueTexture = this.Content.Load<Texture2D>("Balloons/blue200");
            _greenTexture = this.Content.Load<Texture2D>("Balloons/green200");
            _debugFont = this.Content.Load<SpriteFont>("Text");

            // Populate pool.
            Balloon b;
            while (_spawnCount < 5)
            {
                b = new Balloon();
                _balloonMemory.AddFirst(b);
                _spawnCount++;
            }
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

            int index = 0;
            Balloon b;
            while (index < _balloons.Count)
            {
                b = _balloons[index];

                switch (b.State)
                {
                    case BalloonState.Alive:
                        {
                            b.Update(gameTime);
                            index++;
                        }
                        break;
                    case BalloonState.Dead:
                        {
                            b.Uninitialize();
                            _balloonMemory.AddFirst(b);
                            _balloons.RemoveAt(index);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (_spawnTimer.Update(gameTime))
            {
                TimerElapsed();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            int index = 0;
            while (index < _balloons.Count)
            {
                _balloons[index].Draw(ref _spriteBatch);
                index++;
            }

            _spriteBatch.DrawString(_debugFont, "Memory Pool: " + _balloonMemory.Count, Vector2.Zero, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SpawnRedBalloon()
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
            int x = _randomPosition.Next(_screenWidth - _redTexture.Width);
            spawn.Colour = BalloonColour.Red;
            spawn.Initialize(ref _redTexture, new Vector2(x, _screenHeight), _spawnVelocity, 0.5f);
            _balloons.Add(spawn);
        }

        private void TimerElapsed()
        {
            SpawnRedBalloon();
        }
    }
}
