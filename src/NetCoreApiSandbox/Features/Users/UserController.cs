namespace NetCoreApiSandbox.Features.Users
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Security;

    #endregion

    [Route("user")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class UserController
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMediator _mediator;

        public UserController(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
        {
            this._mediator = mediator;
            this._currentUserAccessor = currentUserAccessor;
        }

        [HttpGet]
        public async Task<UserEnvelope> GetCurrent()
        {
            return await this._mediator.Send(new Details.Query
            {
                Username = this._currentUserAccessor.GetCurrentUsername()
            });
        }

        [HttpPut]
        public async Task<UserEnvelope> UpdateUser([FromBody] Edit.Command command)
        {
            return await this._mediator.Send(command);
        }
    }
}
