namespace NetCoreApiSandbox.Domain
{
    #region

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The article entity
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Gets or sets the article identifier.
        /// </summary>
        /// <value>The article identifier.</value>
        [JsonIgnore]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>The slug.</value>
        public string Slug { get; set; }

        /// <summary>
        /// Get or sets article title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Get or sets article description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get or sets article body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Author navigation property
        /// </summary>
        [ForeignKey("AuthorId")]
        public Person Author { get; set; }

        /// <summary>
        /// Onw to many comments navigation property
        /// </summary>
        public ICollection<Comment> Comments { get; set; }

        /// <summary>
        /// Not used property TODO
        /// </summary>
        [NotMapped]
        public bool Favorited => this.ArticleFavorites?.Any() ?? false;

        /// <summary>
        /// Not used property TODO
        /// </summary>
        [NotMapped]
        public int FavoritesCount => this.ArticleFavorites?.Count ?? 0;

        /// <summary>
        /// Gets tag list with using ArticleTags table
        /// </summary>
        [NotMapped]
        public IEnumerable<string> TagList => (this.ArticleTags?.Select(x => x.TagId) ?? Enumerable.Empty<string>());

        /// <summary>
        /// Many to many tags navigation property
        /// </summary>
        [JsonIgnore]
        public IEnumerable<ArticleTag> ArticleTags { get; set; }

        /// <summary>
        /// Many to many person's favorite navigation property
        /// </summary>
        [JsonIgnore]
        public ICollection<ArticleFavorite> ArticleFavorites { get; set; }

        /// <summary>
        /// Gets or sets the date article is created at
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date article is updated at
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
