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

namespace sandbox1
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

        private SpawnTimer _spawnTimer;
        private int _spawnCount;
        private Vector2 _spawnVelocity;
        private Random _randomPosition;

        private LinkedList<Balloon> _balloonMemory;
        private List<Balloon> _balloons;

        private SpriteFont _debugFont;

        private struct Timer
        {
            public float TimePassed;
            public float TimeToSpawn;
        }
        private Timer _myTimer;

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
            //TimeSpan time = new TimeSpan(0,0,4);
            //_spawnTimer = new SpawnTimer(time);
            //_spawnTimer.Elapsed += TimerElapsed;

            _myTimer = new Timer() { TimePassed = 0f, TimeToSpawn = 15000f };

            _spawnCount = 0;
            _spawnVelocity = new Vector2(0, -7);
            _randomPosition = new Random(DateTime.Now.Millisecond);

            //TouchPanel.EnabledGestures = GestureType.None;

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
            while (_spawnCount < 10)
            {
                b = new Balloon("Balloon" + _spawnCount++, ref _redTexture);
                b.Velocity = _spawnVelocity;
                _balloonMemory.AddFirst(b);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            //if (TouchPanel.IsGestureAvailable)
            //{
            //    this.Exit();
            //}          

            int index = 0;
            while (index < _balloons.Count)
            {
                _balloons[index].Update(gameTime);
                index++;
            }

            index = 0;
            Balloon b;
            while (index < _balloons.Count)
            {
                b = _balloons[index];
                if (b.Position.Y < 0)
                {
                    _balloonMemory.AddFirst(b);
                    _balloons.RemoveAt(index);
                }
                else
                {
                    index++;
                }                
            }

            _myTimer.TimePassed += gameTime.TotalGameTime.Milliseconds;

            if (_myTimer.TimePassed >= _myTimer.TimeToSpawn)
            {
                _myTimer.TimePassed %= _myTimer.TimeToSpawn;
                TimerElapsed();
            }

            //_spawnTimer.Update(gameTime);

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

        private void TimerElapsed()
        {
            Balloon spawn;

            if (_balloonMemory.Count > 0)
            {
                spawn = _balloonMemory.First.Value;
                _balloonMemory.RemoveFirst();
            }
            else
            {
                spawn = new Balloon("Balloon" + _spawnCount, ref _redTexture);
                spawn.Colour = BalloonColour.Red;
                spawn.Velocity = _spawnVelocity;
                _spawnCount++;
            }

            // Next position from 0 to the visible edge of the texture.
            int x = _randomPosition.Next(_screenWidth - _redTexture.Width);
            spawn.SetPosition(x,_screenHeight);

            _balloons.Add(spawn);
        }
    }
}
