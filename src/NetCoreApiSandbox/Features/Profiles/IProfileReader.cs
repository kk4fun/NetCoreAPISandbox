namespace NetCoreApiSandbox.Features.Profiles
{
    #region

    using System.Threading.Tasks;

    #endregion

    public interface IProfileReader
    {
        Task<ProfileEnvelope> ReadProfile(string username);
    }
}
