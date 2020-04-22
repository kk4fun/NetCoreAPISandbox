namespace NetCoreApiSandbox.Features.Profiles
{
    /// <summary>
    /// Profile envelop class
    /// </summary>
    public class ProfileEnvelope
    {
        public ProfileEnvelope(Profile profile)
        {
            this.Profile = profile;
        }

        public Profile Profile { get; set; }
    }
}
