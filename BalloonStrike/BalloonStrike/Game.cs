using System;
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using GameCore;
using GameInterfaceFramework;
using BalloonStrike.Views;


namespace BalloonStrike
{
    /// <summary>
    /// This is the sandbox environment generator to test and create new things!
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private ViewManager _viewManager;

        public Game()
        {
            Content.RootDirectory = "Content";

            InitializeGraphics();
            InitializePhoneServices();
            InitializeGameServices();
        }

        protected override void LoadContent()
        {
            InitializeResources();

            base.LoadContent();
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
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
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

            ViewFactory viewFactory = new ViewFactory();
            Services.AddService(typeof(IViewFactory), viewFactory);

            _viewManager = new ViewManager(this);
            Components.Add(_viewManager);
        }

        private void InitializeResources()
        {
            ResourceManager.Initialize(Content);
        }

        private void InitializeGame()
        {
            _viewManager.AddView(new SplashView());
        }

        private void GameActivated(object sender, ActivatedEventArgs e)
        {
            // If rehydrating, reload resources.
            if (!e.IsApplicationInstancePreserved)
            {
                InitializeResources();
            }

            // If we fail to rehydrate then restart the game.
            if (!_viewManager.Activate(e.IsApplicationInstancePreserved))
            //if (!_viewManager.Activate(false))
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
    }
}
