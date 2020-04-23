namespace NetCoreApiSandbox.Domain
{
    #region

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion

    /// <summary>
    /// The tag entity
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Gets or sets thje tag identifier
        /// </summary>
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// Many to many navigation property
        /// </summary>
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
