namespace NetCoreApiSandbox.Domain
{
    #region

    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The comment entity
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets or sets the comment id
        /// </summary>
        [JsonProperty("id")]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the comment body            
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The author inverse navigation property
        /// </summary>
        public User Author { get; set; }

        /// <summary>
        /// Author id foreign key
        /// </summary>
        [JsonIgnore]
        public int AuthorId { get; set; }

        /// <summary>
        /// The article inverse navigation property
        /// </summary>
        [JsonIgnore]
        public Article Article { get; set; }

        /// <summary>
        /// Article id foreign key
        /// </summary>
        [JsonIgnore]
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the date comment was created at
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date comment has been updated at
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
