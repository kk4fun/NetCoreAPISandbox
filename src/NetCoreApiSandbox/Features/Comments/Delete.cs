namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public class Delete
    {
        #region Nested type: Command

        public class Command: IRequest
        {
            public Command(string slug, int id)
            {
                this.Slug = slug;
                this.Id = id;
            }

            public string Slug { get; }

            public int Id { get; }
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
                var article = await this._context.Articles.Include(x => x.Comments)
                                        .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                var comment = article.Comments.FirstOrDefault(x => x.Id == message.Id);

                if (comment == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Comment = Constants.NotFound });
                }

                this._context.Comments.Remove(comment);
                await this._context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            #endregion
        }

        #endregion
    }
}
