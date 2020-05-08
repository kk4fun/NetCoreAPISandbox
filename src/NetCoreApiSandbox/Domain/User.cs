namespace NetCoreApiSandbox.Domain
{
    #region

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Registered user entity
    /// </summary>
    [Table("Users")]
    public class User
    {
        private byte[] _hash;
        private byte[] _salt;

        /// <summary>
        /// Get or sets primary key
        /// </summary>
        [JsonIgnore]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Get or sets username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Get or sets email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Get or sets user bio
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Get or sets avatar
        /// </summary>
        public string Image { get; set; }

        public Person Person { get; set; }

        /// <summary>
        /// Navigation property for the article favorites 
        /// </summary>
        [JsonIgnore]
        public ICollection<ArticleFavorite> ArticleFavorites { get; set; }

        /// <summary>
        /// Navigation property for the persons who follow current person
        /// </summary>
        [JsonIgnore]
        public ICollection<FollowedPeople> Following { get; set; }

        /// <summary>
        /// Navigation property for the persons who followed by current person
        /// </summary>
        [JsonIgnore]
        public ICollection<FollowedPeople> Followers { get; set; }

        /// <summary>
        /// Get or sets password hash
        /// </summary>
        [JsonIgnore]
        public byte[] Hash
        {
            get => (byte[]) this._hash.Clone();
            set => this._hash = value;
        }

        /// <summary>
        /// Gets or sets password salt
        /// </summary>
        [JsonIgnore]
        public byte[] Salt
        {
            get => (byte[]) this._salt.Clone();
            set => this._salt = value;
        }
    }
}
