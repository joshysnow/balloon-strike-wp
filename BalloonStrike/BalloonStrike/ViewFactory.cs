using System;
using GameInterfaceFramework;

namespace BalloonStrike
{
    public class ViewFactory : IViewFactory
    {
        public View CreateView(Type viewType)
        {
            return Activator.CreateInstance(viewType) as View;
        }
    }
}