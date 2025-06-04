
namespace BussinessLogic
{

    /// <summary>
    /// Specifies properties or collections to exclude during object mapping operations in custom mapping contexts.
    /// </summary>
    public enum MappingContextExclusion
    {
        /// <summary>
        /// Exclude notes during mapping.
        /// </summary>
        Notes,

        /// <summary>
        /// Exclude activities during mapping.
        /// </summary>
        Activities,

        /// <summary>
        /// Exclude memory files (souvenirs) during mapping.
        /// </summary>
        Memories,

        /// <summary>
        /// Exclude follower information during mapping.
        /// </summary>
        Followers
    }
}
