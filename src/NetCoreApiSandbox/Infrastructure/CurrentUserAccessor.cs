namespace NetCoreApiSandbox.Infrastructure
{
    #region

    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    #endregion

    public class CurrentUserAccessor: ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        #region ICurrentUserAccessor Members

        public string GetCurrentUsername()
        {
            return this._httpContextAccessor.HttpContext.User?.Claims
                      ?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                      ?.Value;
        }

        #endregion
    }
}
