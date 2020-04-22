namespace NetCoreApiSandbox.Features.Profiles
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    [Route("profiles")]
    public class ProfilesController: Controller
    {
        private readonly IMediator _mediator;

        public ProfilesController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("{username}")]
        public async Task<ProfileEnvelope> Get(string username)
        {
            return await this._mediator.Send(new Details.Query() { Username = username });
        }
    }
}
