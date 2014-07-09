using System;
using Microsoft.Xna.Framework;

namespace GameInterfaceFramework
{
    public class LoadView : View
    {
        private View[] _newViews;
        private byte _remainingViews;

        private LoadView(byte remainingViews, params View[] viewsToLoad)
        {
            _remainingViews = remainingViews;
            _newViews = viewsToLoad;
        }

        public static void Load(ViewManager manager, byte numberOfViews, params View[] viewsToLoad)
        {
            // Add 1 to include this view.
            byte viewCount = (byte)(manager.Views().Length + 1);

            // Take off the views that are exiting.
            viewCount -= numberOfViews;

            // Add this view to the stack.
            manager.AddView(new LoadView(viewCount, viewsToLoad));
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            // When the required views have exited, load in the new views.
            if (ViewManager.Views().Length == _remainingViews)
            {
                // Add all the views to the game.
                foreach (View view in _newViews)
                {
                    ViewManager.AddView(view);
                }

                // Kill this view.
                Exit();
            }

            base.Update(gameTime, covered);
        }
    }
}
