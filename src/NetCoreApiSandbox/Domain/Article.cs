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
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>The slug.</value>
        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public Person Author { get; set; }

        public List<Comment> Comments { get; set; }

        [NotMapped]
        public bool Favorited => this.ArticleFavorites?.Any() ?? false;

        [NotMapped]
        public int FavoritesCount => this.ArticleFavorites?.Count ?? 0;

        [NotMapped]
        public List<string> TagList => (this.ArticleTags?.Select(x => x.TagId) ?? Enumerable.Empty<string>()).ToList();

        [JsonIgnore]
        public List<ArticleTag> ArticleTags { get; set; }

        [JsonIgnore]
        public List<ArticleFavorite> ArticleFavorites { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
