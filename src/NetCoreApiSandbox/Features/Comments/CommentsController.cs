namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NetCoreApiSandbox.Infrastructure.Security;

    #endregion

    [Route("articles")]
    public class CommentsController: Controller
    {
        private readonly IMediator _mediator;

        public CommentsController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost("{slug}/comments")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<CommentEnvelope> Create(string slug, [FromBody] Create.Command command)
        {
            command.Slug = slug;

            return await this._mediator.Send(command);
        }

        [HttpGet("{slug}/comments")]
        public async Task<CommentsEnvelope> Get(string slug)
        {
            return await this._mediator.Send(new List.Query(slug));
        }

        [HttpDelete("{slug}/comments/{id}")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task Delete(string slug, int id)
        {
            await this._mediator.Send(new Delete.Command(slug, id));
        }
    }
}
