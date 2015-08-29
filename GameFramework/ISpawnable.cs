
namespace GameFramework
{
    /// <summary>
    /// Used for the Spawner to gather necessary information
    /// that may be encapsulated or needed for particular functionality.
    /// </summary>
    public interface ISpawnable
    {
        /// <summary>
        /// Children must implement property for dehydration purposes.
        /// An object may not be known or is generic so a Spawner for
        /// instance will not be able to determine the objects class so,
        /// to maintain encapsulation this property is given to all Spawnable
        /// objects to determine for themselves. It is a string as to
        /// not be so strict as to rely on a enum of spawnable objects which,
        /// makes the Spawner more portable to other projects without having
        /// to alter an enum.
        /// </summary>
        string SpawnType
        {
            get;
        }
    }
}
