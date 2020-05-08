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

    public static class Delete
    {
        #region Nested type: Command

        public class Command: IRequest
        {
            public Command(string slug)
            {
                this.Slug = slug;
            }

            public string Slug { get; }
        }

        #endregion

        #region Nested type: CommandValidator

        private class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Command>
        {
            private readonly NetCoreSandboxApiContext _context;

            public QueryHandler(NetCoreSandboxApiContext context)
            {
                this._context = context;
            }

            #region IRequestHandler<Command> Members

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var article =
                    await this._context.Articles.FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                this._context.Articles.Remove(article);
                await this._context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            #endregion
        }

        #endregion
    }
}
