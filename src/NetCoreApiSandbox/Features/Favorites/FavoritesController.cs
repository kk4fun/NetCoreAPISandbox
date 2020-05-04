namespace NetCoreApiSandbox.Features.Favorites
{
    #region

    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NetCoreApiSandbox.Features.Articles;
    using NetCoreApiSandbox.Infrastructure.Security;

    #endregion

    [Route("articles")]
    public class FavoritesController: Controller
    {
        private readonly IMediator _mediator;

        public FavoritesController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpPost("{slug}/favorite")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ArticleEnvelope> FavoriteAdd(string slug)
        {
            return await this._mediator.Send(new Add.Command(slug));
        }

        [HttpDelete("{slug}/favorite")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<ArticleEnvelope> FavoriteDelete(string slug)
        {
            return await this._mediator.Send(new Delete.Command(slug));
        }
    }
}
