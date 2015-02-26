using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameInterfaceFramework
{
    public class ViewManager : DrawableGameComponent
    {
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public GestureType EnabledGestures
        {
            get { return TouchPanel.EnabledGestures; }
            set { TouchPanel.EnabledGestures = value; }
        }

        private const string STORAGE_FILE_NAME = "ViewManagerState.xml";

        private SpriteBatch _spriteBatch;
        private List<View> _views;
        private List<View> _tempViews;
        private ControlsState _controls;

        public ViewManager(Game game) : base(game)
        {
            _views = new List<View>();
            _tempViews = new List<View>();
            _controls = new ControlsState();

            EnabledGestures = GestureType.None;
        }

        public void AddView(View newView, bool rehydrate = false)
        {
            newView.ViewManager = this;
            newView.Activate(rehydrate);
            _views.Add(newView);

            EnabledGestures = newView.ViewGestures;
        }

        public void RemoveView(View view)
        {
            _views.Remove(view);

            EnabledGestures = _views.Last().ViewGestures;
        }

        /// <summary>
        /// Get a copy of the views. Useful for
        /// calculating how many 
        /// </summary>
        /// <returns>An array of all views.</returns>
        public View[] Views()
        {
            return _views.ToArray();
        }

        public bool Activate(bool instancePreserved)
        {
            if (instancePreserved)
            {
                // Copy the master view list in case activating a view
                // removes or adds new views.
                _tempViews.Clear();
                _tempViews.AddRange(_views);

                foreach (View view in _tempViews)
                {
                    view.Activate(instancePreserved);
                }
            }
            else
            {
                IViewFactory viewFactory = Game.Services.GetService(typeof(IViewFactory)) as IViewFactory;

                if (viewFactory == null)
                {
                    throw new InvalidOperationException("A IViewFactory object couldn't be found in Game.Services.");
                }

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!storage.FileExists(STORAGE_FILE_NAME))
                        return false;

                    using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);

                        foreach (XElement viewElement in doc.Root.Elements("View"))
                        {
                            Type viewType = Type.GetType(viewElement.Attribute("Type").Value);
                            View view = viewFactory.CreateView(viewType);

                            AddView(view, true);
                        }
                    }
                }
            }

            return true;
        }

        public void Deactivate()
        {
            // Create the file in storage usable by the game.
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("ViewManager");
                doc.Add(root);

                // Copy the master view list incase one View
                // deactivates another.
                _tempViews.Clear();
                _tempViews.AddRange(_views);

                XElement viewElement;

                foreach (View view in _tempViews)
                {
                    if (view.IsSerializable)
                    {
                        viewElement = new XElement(
                            "View", 
                            new XAttribute("Type", view.GetType().AssemblyQualifiedName)
                            );

                        root.Add(viewElement);
                    }

                    view.Deactivate();
                }

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _controls.Update();
            UpdateViews(gameTime);            
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (View view in _views)
            {
                if (view.State == TransitionState.Hidden)
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
            _tempViews.Reverse();

            View view;
            byte index = 0;
            bool isActiveApp = Game.IsActive;
            bool hasFocus = true;
            bool covered = false;

            while (index < _tempViews.Count)
            {
                view = _tempViews[index];

                view.Update(gameTime, covered);

                if ((view.State == TransitionState.Active) || (view.State == TransitionState.TransitionOn))
                {
                    if (isActiveApp && hasFocus)
                    {
                        view.HandlePlayerInput(_controls);
                        hasFocus = false;
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
