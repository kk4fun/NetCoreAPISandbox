namespace NetCoreApiSandbox.Domain
{
    #region

    using System.Collections.Generic;
    using Newtonsoft.Json;

    #endregion

    public class Person
    {
        private byte[] _hash;

        [JsonIgnore]
        public int PersonId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        [JsonIgnore]
        public List<ArticleFavorite> ArticleFavorites { get; set; }

        [JsonIgnore]
        public List<FollowedPeople> Following { get; set; }

        [JsonIgnore]
        public List<FollowedPeople> Followers { get; set; }

        [JsonIgnore]
        public byte[] Hash
        {
            get => (byte[]) this._hash.Clone();
            set => this._hash = value;
        }

        [JsonIgnore]
        public byte[] Salt { get; set; }
    }
}
