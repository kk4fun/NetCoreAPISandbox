namespace NetCoreApiSandbox.Features.Profiles
{
    #region

    using Newtonsoft.Json;

    #endregion

    public class Profile
    {
        public string Username { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        [JsonProperty("following")]
        public bool IsFollowed { get; set; }
    }
}
