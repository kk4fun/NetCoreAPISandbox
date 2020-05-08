namespace NetCoreApiSandbox.Domain
{
    /// <summary>
    /// Followed people entity
    /// </summary>
    public class FollowedPeople
    {
        /// <summary>
        /// Gets or sets the observer identifier
        /// </summary>
        public int ObserverId { get; set; }

        /// <summary>
        /// Observer navigation property
        /// </summary>
        public User Observer { get; set; }

        /// <summary>
        /// Get or sets the target identifier
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Target navigation property
        /// </summary>
        public User Target { get; set; }
    }
}
