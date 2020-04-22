namespace NetCoreApiSandbox.Infrastructure.Security
{
    #region

    using System.Threading.Tasks;

    #endregion

    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(string username);
    }
}
