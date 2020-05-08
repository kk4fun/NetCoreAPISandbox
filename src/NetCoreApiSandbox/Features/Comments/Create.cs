namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public class Create
    {
        #region Nested type: Command

        public class Command: IRequest<CommentEnvelope>
        {
            public CommentData Comment { get; set; }

            public string Slug { get; set; }
        }

        #endregion

        #region Nested type: CommandValidator

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.Comment).NotNull().SetValidator(new CommentDataValidator());
            }
        }

        #endregion

        #region Nested type: CommentData

        public class CommentData
        {
            public string Body { get; set; }
        }

        #endregion

        #region Nested type: CommentDataValidator

        public class CommentDataValidator: AbstractValidator<CommentData>
        {
            public CommentDataValidator()
            {
                this.RuleFor(x => x.Body).NotNull().NotEmpty();
            }
        }

        #endregion

        #region Nested type: Handler

        public class Handler: IRequestHandler<Command, CommentEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(NetCoreSandboxApiContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this._context = context;
                this._currentUserAccessor = currentUserAccessor;
            }

            #region IRequestHandler<Command,CommentEnvelope> Members

            public async Task<CommentEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await this._context.Articles.Include(x => x.Comments)
                                        .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                var author =
                    await this._context.Users.FirstAsync(x => x.Username ==
                                                              this._currentUserAccessor.GetCurrentUsername(),
                                                         cancellationToken);

                var comment = new Comment
                {
                    Author = author,
                    Body = message.Comment.Body,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await this._context.Comments.AddAsync(comment, cancellationToken);

                article.Comments.Add(comment);

                await this._context.SaveChangesAsync(cancellationToken);

                return new CommentEnvelope(comment);
            }

            #endregion
        }

        #endregion
    }
}
