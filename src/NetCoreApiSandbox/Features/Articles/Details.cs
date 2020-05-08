namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public static class Details
    {
        #region Nested type: Query

        public class Query: IRequest<ArticleEnvelope>
        {
            public Query(string slug)
            {
                this.Slug = slug;
            }

            public string Slug { get; set; }
        }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Query, ArticleEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;

            public QueryHandler(NetCoreSandboxApiContext context)
            {
                this._context = context;
            }

            #region IRequestHandler<Query,ArticleEnvelope> Members

            public async Task<ArticleEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var article = await this._context.Articles.GetAllData()
                                        .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                return new ArticleEnvelope(article);
            }

            #endregion
        }

        #endregion

        #region Nested type: QueryValidator

        public class QueryValidator: AbstractValidator<Query>
        {
            public QueryValidator()
            {
                this.RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        #endregion
    }
}
