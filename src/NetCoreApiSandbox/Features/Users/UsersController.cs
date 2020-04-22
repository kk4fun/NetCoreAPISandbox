namespace NetCoreApiSandbox.Features.Users
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    [Route("users")]
    public class UsersController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="command">register command</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command)
        {
            return await this._mediator.Send(command);
        }

        [HttpPost("login")]
        public async Task<UserEnvelope> Login([FromBody] Login.Command command)
        {
            return await this._mediator.Send(command);
        }
    }
}
