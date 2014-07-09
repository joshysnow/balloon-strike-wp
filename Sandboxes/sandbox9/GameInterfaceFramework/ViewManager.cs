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
            newView.ViewManager = this;
            newView.Activate(false);

            _views.Push(newView);

            TouchPanel.EnabledGestures = newView.EnabledGestures;
        }

        public View[] Views()
        {
            return _views.ToArray();
        }

        public bool Activate(bool instancePreserved)
        {
#warning TODO: Deserialize the game from disk.
            return false;
        }

        public void Deactivate()
        {
#warning TODO: Serialize the game to disk.
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            TrimDeadViews();
            UpdateViews(gameTime);            
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

        private void TrimDeadViews()
        {
            byte index = 0;
            View view;
            List<View> views = new List<View>(Views());

            while (index < views.Count)
            {
                view = views[index];

                if (view.IsExiting && (view.State == ViewState.Hidden))
                    views.RemoveAt(index);
                else
                    index++;
            }

            _views = new Stack<View>(views);
        }

        private void UpdateViews(GameTime gameTime)
        {
            bool isActiveApp = !Game.IsActive;
            bool covered = false;

            View[] viewsCopy = Views();

            foreach (View view in viewsCopy)
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
    }
}
