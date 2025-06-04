namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents an individual follower with a unique identifier and personal name details.
    /// </summary>
    public class Follower
    {
        /// <summary>
        /// Gets or sets the unique identifier for the follower.
        /// </summary>
        public int FollowerID { get; set; }

        /// <summary>
        /// Gets or sets the first name of the follower.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name (surname) of the follower, if available.
        /// </summary>
        public string? LastName { get; set; }
    }
}