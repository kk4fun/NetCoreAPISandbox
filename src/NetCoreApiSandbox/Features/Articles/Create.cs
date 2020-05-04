namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Infrastructure;

    #endregion

    public class Create
    {
        #region Nested type: ArticleDataValidator

        private class ArticleDataValidator: AbstractValidator<ArticleDTO>
        {
            public ArticleDataValidator()
            {
                this.RuleFor(x => x.Title).NotNull().NotEmpty();
                this.RuleFor(x => x.Description).NotNull().NotEmpty();
                this.RuleFor(x => x.Body).NotNull().NotEmpty();
            }
        }

        #endregion

        #region Nested type: Command

        public class Command: IRequest<ArticleEnvelope>
        {
            public ArticleDTO Article { get; set; }
        }

        #endregion

        #region Nested type: CommandValidator

        private protected class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.Article).NotNull().SetValidator(new ArticleDataValidator());
            }
        }

        #endregion

        #region Nested type: Handler

        public class Handler: IRequestHandler<Command, ArticleEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(NetCoreSandboxApiContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this._context = context;
                this._currentUserAccessor = currentUserAccessor;
            }

            #region IRequestHandler<Command,ArticleEnvelope> Members

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var author =
                    await this._context.Persons.FirstAsync(x => x.Username ==
                                                                this._currentUserAccessor.GetCurrentUsername(),
                                                           cancellationToken);

                var tags = new List<Tag>();

                foreach (var tag in message.Article.TagList ?? Enumerable.Empty<string>())
                {
                    var t = await this._context.Tags.FindAsync(tag);

                    if (t == null)
                    {
                        t = new Tag { Id = tag };

                        await this._context.Tags.AddAsync(t, cancellationToken);

                        //save immediately for reuse
                        await this._context.SaveChangesAsync(cancellationToken);
                    }

                    tags.Add(t);
                }

                var article = new Article
                {
                    Author = author,
                    Body = message.Article.Body,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Description = message.Article.Description,
                    Title = message.Article.Title,
                    Slug = message.Article.Title.GenerateSlug()
                };

                await this._context.Articles.AddAsync(article, cancellationToken);

                await this._context.ArticleTags.AddRangeAsync(tags.Select(x => new ArticleTag
                                                              {
                                                                  Article = article, Tag = x
                                                              }),
                                                              cancellationToken);

                await this._context.SaveChangesAsync(cancellationToken);

                return new ArticleEnvelope(article);
            }

            #endregion
        }

        #endregion

        // #region Nested type: ArticleDTO
        //
        // /// <summary>
        // /// Class that contains article data
        // /// </summary>
        // public class ArticleDTO
        // {
        //     public string Title { get; set; }
        //
        //     public string Description { get; set; }
        //
        //     public string Body { get; set; }
        //
        //     public IEnumerable<string> TagList { get; set; }
        // }
        //
        // #endregion
    }
}
