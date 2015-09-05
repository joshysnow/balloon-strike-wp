using System;
using BalloonStrike.Views;
using GameInterfaceFramework;

namespace BalloonStrike
{
    public class ViewFactory : IViewFactory
    {
        public View CreateView(Type viewType, bool rehydrated = false)
        {
            return Activator.CreateInstance(viewType, rehydrated) as View;
        }
    }
}