namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public class List
    {
        #region Nested type: Query

        public class Query: IRequest<CommentsEnvelope>
        {
            public Query(string slug)
            {
                this.Slug = slug;
            }

            public string Slug { get; }
        }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Query, CommentsEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;

            public QueryHandler(NetCoreSandboxApiContext context)
            {
                this._context = context;
            }

            #region IRequestHandler<Query,CommentsEnvelope> Members

            public async Task<CommentsEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var article = await this._context.Articles.Include(x => x.Comments)
                                        .ThenInclude(x => x.Author)
                                        .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                return new CommentsEnvelope(article.Comments);
            }

            #endregion
        }

        #endregion
    }
}
