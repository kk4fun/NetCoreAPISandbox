namespace NetCoreApiSandbox.Features.Favorites
{
    #region

    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Features.Articles;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public sealed class Add
    {
        #region Nested type: Command

        public class Command: IRequest<ArticleEnvelope>
        {
            public Command(string slug)
            {
                this.Slug = slug;
            }

            public string Slug { get; }
        }

        #endregion

        #region Nested type: CommandValidator

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Command, ArticleEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public QueryHandler(NetCoreSandboxApiContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this._context = context;
                this._currentUserAccessor = currentUserAccessor;
            }

            #region IRequestHandler<Command,ArticleEnvelope> Members

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article =
                    await this._context.Articles.FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                var person =
                    await this._context.Users.FirstOrDefaultAsync(x => x.Username ==
                                                                       this._currentUserAccessor.GetCurrentUsername(),
                                                                  cancellationToken);

                var favorite =
                    await this._context.ArticleFavorites.FirstOrDefaultAsync(x => x.ArticleId == article.Id &&
                                                                                  x.UserId == person.Id,
                                                                             cancellationToken);

                if (favorite == null)
                {
                    favorite = new ArticleFavorite
                    {
                        Article = article, ArticleId = article.Id, User = person, UserId = person.Id
                    };

                    await this._context.ArticleFavorites.AddAsync(favorite, cancellationToken);
                    await this._context.SaveChangesAsync(cancellationToken);
                }

                return new ArticleEnvelope(await this._context.Articles.GetAllData()
                                                     .FirstOrDefaultAsync(x => x.Id == article.Id, cancellationToken));
            }

            #endregion
        }

        #endregion
    }
}
