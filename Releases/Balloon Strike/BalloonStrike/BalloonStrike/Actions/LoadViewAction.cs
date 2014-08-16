using GameCore;
using GameInterfaceFramework;

namespace GameInterfaceFramework.Actions
{
    public class LoadViewAction : IActionHandler
    {
        private View _actionView;
        private View _newView;
        private byte _numberOfViews;

        public LoadViewAction(byte currentViews, View actionView, View viewToLoad)
        {
            _numberOfViews = currentViews;
            _actionView = actionView;
            _newView = viewToLoad;
        }

        public void Execute()
        {
            LoadView.Load(_actionView.ViewManager, _numberOfViews, _newView);
        }
    }
}
