using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameInterfaceFramework;

namespace BalloonStrike.Views
{
    public class InfoView : View
    {
        private string[] CREDITS = new string[12] 
        { 
            "Project Lead", "Joshua Hirst", 
            "Producer", "Govinda Singh",
            "Programmer", "Joshua Hirst",
            "Artist", "Abigail Royle",
            "Artist", "Mark Hallinan",
            "Sound", "William Voce"
        };

        private string[] SMALL_PRINT = new string[3]
        {
            "Balloon Strike Version ",
            "Twitter: @foxcodestudios",
            "FoxCode Studios. All rights reserved 2014"
        };

        private CreditsPlayer _creditsPlayer;
        private SpriteFont _smallPrintFont;
        private Vector2[] _indices;

        public InfoView(bool rehydrated = false)
            :base(rehydrated)
        {
            Transition.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            Transition.TransitionOffTime = TimeSpan.FromSeconds(0.5);

            ViewGestures = GestureType.Tap;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GraphicsDevice graphics = ViewManager.GraphicsDevice;

                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;

                ResourceManager resources = ResourceManager.Resources;
                _smallPrintFont = resources.GetFont("small");
                SpriteFont titleFont = resources.GetFont("credit_title");
                SpriteFont nameFont = resources.GetFont("credit_name");
                CreditModel creditModel = new CreditModel(titleFont, nameFont);

                _creditsPlayer = new CreditsPlayer();

                Vector2 titlePosition;
                Vector2 namePosition;

                Vector2 titleSize;
                Vector2 nameSize;

                string title;
                string name;

                Credit tempCredit;

                byte index = 0;
                while(index < CREDITS.Length)
                {
                    title = CREDITS[index++];
                    name = CREDITS[index++];

                    titleSize = titleFont.MeasureString(title);
                    nameSize = nameFont.MeasureString(name);
                    titlePosition = new Vector2((width - titleSize.X) / 2, (height / 2) - titleSize.Y);
                    namePosition = new Vector2((width - nameSize.X) / 2, (height / 2));

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
                SMALL_PRINT[0] += info[VERSION].Trim().Split('=')[VERSION];

                _indices = new Vector2[SMALL_PRINT.Length];

                // Space from the bottom of the screen in pixels.
                const byte SPACING = 10;
                int step = (int)_smallPrintFont.MeasureString("l").Y;
                int totalHeight = step * SMALL_PRINT.Length;
                int y = (graphics.Viewport.Height - SPACING) - totalHeight;

                string tempInfo;
                Vector2 tempSize;
                Vector2 tempPosition;

                for (int i = 0; i < _indices.Length; i++)
                {
                    tempInfo = SMALL_PRINT[i];
                    tempSize = _smallPrintFont.MeasureString(tempInfo);
                    tempPosition = new Vector2((graphics.Viewport.Width - tempSize.X) / 2, 
                        (i == 0) ? y : y + (step * i));

                    _indices[i] = tempPosition;
                }
            }
        }

        public override void HandlePlayerInput(ControlsState controls)
        {
            if (controls.BackButtonPressed())
            {
                _creditsPlayer.End(Transition.TransitionOffTime);
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
            _creditsPlayer.Draw(spriteBatch);

            // Draw info.
            byte index = 0;
            while (index < SMALL_PRINT.Length)
            {
                spriteBatch.DrawString(_smallPrintFont, SMALL_PRINT[index], _indices[index], Color.Black * TransitionAlpha);
                index++;
            }

            spriteBatch.End();
        }
    }
}
