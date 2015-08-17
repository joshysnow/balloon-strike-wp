using System;

namespace GameInterfaceFramework
{
    /// <summary>
    /// A view factory should be used to rehydrate a view from storage.
    /// 
    /// Tombstoning is handled by XML and this interface will be passed
    /// a type and be able to create that screen. 
    /// 
    /// Using this interface means that a view maybe constructed differently
    /// depending on per implementation by, creating a new ViewFactory object
    /// inheriting this.
    /// </summary>
    public interface IViewFactory
    {
        View CreateView(Type viewType);
    }
}
