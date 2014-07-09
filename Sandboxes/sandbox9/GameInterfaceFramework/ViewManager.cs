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

        private SpriteBatch _spriteBatch;
        private List<View> _views;
        private List<View> _tempViews;

        public ViewManager(Game game) : base(game)
        {
            _views = new List<View>();
            _tempViews = new List<View>();

            TouchPanel.EnabledGestures = GestureType.None;
        }

        public void AddView(View newView)
        {
            newView.ViewManager = this;
            newView.Activate(false);
            _views.Add(newView);

            TouchPanel.EnabledGestures = newView.EnabledGestures;
        }

        public void RemoveView(View view)
        {
            _views.Remove(view);
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

        private void UpdateViews(GameTime gameTime)
        {
            _tempViews.AddRange(_views);
            View view;
            byte index = 0;
            bool isActiveApp = !Game.IsActive;
            bool covered = false;

            while (index < _tempViews.Count)
            {
                view = _tempViews[index];

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

                index++;
            }

            _tempViews.Clear();
        }
    }
}
