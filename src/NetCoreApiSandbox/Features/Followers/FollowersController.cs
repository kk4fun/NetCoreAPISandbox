namespace NetCoreApiSandbox.Features.Followers
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NetCoreApiSandbox.Features.Profiles;
    using NetCoreApiSandbox.Infrastructure.Security;

    #endregion

    [Route("profiles")]
    public class FollowersController: Controller
    {
        private readonly IMediator _mediator;

        public FollowersController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost("{username}/follow")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ProfileEnvelope> Follow(string username)
        {
            return await this._mediator.Send(new Add.Command(username));
        }

        [HttpDelete("{username}/follow")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ProfileEnvelope> Unfollow(string username)
        {
            return await this._mediator.Send(new Delete.Command(username));
        }
    }
}
