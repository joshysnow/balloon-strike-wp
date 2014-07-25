using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class AboutView : View
    {
        private static string[] CREDITS = new string[14] 
        { 
            "Project Lead", "Joshua Hirst", 
            "Producer", "Govinda Singh",
            "Programmer", "Joshua Hirst",
            "Artist", "Abigail Royle",
            "Artist", "Mark Hallinan",
            "Music", "Abigail Royle",
            "Sound", "William Voce"
        };

        private CreditsPlayer _creditsPlayer;
        private SpriteFont _versionFont;
        private Vector2 _versionPosition;
        private string _version;

        public AboutView()
        {
            _transitionOnTime = TimeSpan.FromSeconds(0.5);
            _transitionOffTime = TimeSpan.FromSeconds(0.5);

            _viewGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;

                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;

                ResourceManager resources = ResourceManager.Manager;
                _versionFont = resources.GetFont("debug");

                SpriteFont titleFont = resources.GetFont("credit_title");
                SpriteFont nameFont = resources.GetFont("credit_name");
                CreditModel creditModel = new CreditModel(titleFont, nameFont);

                _creditsPlayer = new CreditsPlayer();

                Vector2 titlePosition;
                Vector2 namePosition;

                string title;
                string name;

                Credit tempCredit;

                byte index = 0;
                while(index < CREDITS.Length)
                {
                    title = CREDITS[index++];
                    name = CREDITS[index++];

                    Vector2 titleSize = titleFont.MeasureString(title);
                    Vector2 nameSize = nameFont.MeasureString(name);
                    titlePosition = new Vector2((width - titleSize.X) / 2, (height / 2) - titleSize.Y);
                    namePosition = new Vector2((width - nameSize.X) / 2, (height / 2) + nameSize.Y);

                    tempCredit = new Credit() 
                    { 
                        Name = name, 
                        Title = title, 
                        TitlePosition = titlePosition, 
                        NamePosition = namePosition, 
                        Model = creditModel 
                    };

                    _creditsPlayer.AddCredit(tempCredit);
                }

                const byte VERSION = 1;
                string[] info = Assembly.GetExecutingAssembly().FullName.Split(',');
                _version = "v" + info[VERSION].Trim().Split('=')[VERSION];
                Vector2 versionSize = _versionFont.MeasureString(_version);
                _versionPosition = new Vector2(width - versionSize.X, height - versionSize.Y);
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                Exit();
                LoadView.Load(ViewManager, 1, new MainMenuView());
            }
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            _creditsPlayer.Update(gameTime);

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ViewManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(_versionFont, _version, _versionPosition, Color.Black * TransitionAlpha);
            _creditsPlayer.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
