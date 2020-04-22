namespace NetCoreApiSandbox.Features.Tags
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    [Route("tags")]
    public class TagsController: Controller
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<TagsEnvelope> Get()
        {
            return await this._mediator.Send(new List.Query());
        }
    }
}
