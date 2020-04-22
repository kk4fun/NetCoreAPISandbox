namespace NetCoreApiSandbox.Features.Profiles
{
    public class ProfileEnvelope
    {
        public ProfileEnvelope(Profile profile)
        {
            this.Profile = profile;
        }

        public Profile Profile { get; set; }
    }
}
