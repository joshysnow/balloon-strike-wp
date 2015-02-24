namespace GameCore
{
    /// <summary>
    /// Provides methods that will mean an object can follow
    /// the pattern to be serialize and handle being rehydrated.
    /// 
    /// So the Initialize() functionshould be used for default
    /// or start-up configuration where by; Activate() used
    /// when the application has resumed. The opposite Deactivate()
    /// function is used to inform an object to store its data.
    /// </summary>
    public interface Serializable
    {
        /// <summary>
        /// Used to initialize an object in default or
        /// start-up mode.
        /// </summary>
        void Initialize();

        /// <summary>
        /// If instance is preserved then attempt to rehydrate.
        /// If that fails or the instance was not preserved, then
        /// Initialize() should be called or some other start-up
        /// values should be loaded/set.
        /// </summary>
        /// <param name="instancePreserved">If the applications memory is rehydratable.</param>
        void Activate(bool instancePreserved);

        /// <summary>
        /// Used to inform an object to begin serialization
        /// as the application is likely or about to be
        /// tombstoned.
        /// </summary>
        void Deactivate();
    }
}