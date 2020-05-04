namespace NetCoreApiSandbox.Domain
{
    /// <summary>
    /// Article-Tag many to many join table
    /// </summary>
    public class ArticleTag
    {
        /// <summary>
        /// The article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Article navigation property
        /// </summary>
        public Article Article { get; set; }

        /// <summary>
        /// The tag identifier
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// Tag navigation property
        /// </summary>
        public Tag Tag { get; set; }
    }
}
