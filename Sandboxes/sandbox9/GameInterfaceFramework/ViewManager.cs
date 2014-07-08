using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameInterfaceFramework
{
    public class ViewManager : DrawableGameComponent
    {
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        private Stack<View> _views;
        private SpriteBatch _spriteBatch;

        public ViewManager(Game game) : base(game)
        {
            _views = new Stack<View>();
            TouchPanel.EnabledGestures = GestureType.None;
        }

        public void AddView(View newView)
        {
            _views.Push(newView);
        }

        public bool Activate(bool instancePreserved)
        {
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            bool isActiveApp = !Game.IsActive;
            bool covered = false;

            foreach (View view in _views)
            {
                view.Update(gameTime, covered);

                if ((view.State == ViewState.Active) || (view.State == ViewState.TransitionOn))
                {
                    if (isActiveApp)
                    {
                        view.HandlePlayerInput();
                    }

                    if (!view.IsPopup)
                    {
                        covered = true;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (View view in _views)
            {
                if (view.State == ViewState.Hidden)
                {
                    continue;
                }

                view.Draw(gameTime);
            }
        }

        protected override void LoadContent()
        {            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }
    }
}
