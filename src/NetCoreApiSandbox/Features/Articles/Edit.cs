namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    public class Edit
    {
        #region Nested type: ArticleData

        public class ArticleData
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public string Body { get; set; }

            public IEnumerable<string> TagList { get; set; }
        }

        #endregion

        #region Nested type: Command

        public class Command: IRequest<ArticleEnvelope>
        {
            public ArticleData Article { get; set; }

            public string Slug { get; set; }
        }

        #endregion

        #region Nested type: CommandValidator

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.Article).NotNull();
            }
        }

        #endregion

        #region Nested type: Handler

        public class Handler: IRequestHandler<Command, ArticleEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;

            public Handler(NetCoreSandboxApiContext context)
            {
                this._context = context;
            }

            #region IRequestHandler<Command,ArticleEnvelope> Members

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await this._context.Articles
                                        .Include(x => x
                                                    .ArticleTags) // include also the article tags since they also need to be updated
                                        .Where(x => x.Slug == message.Slug)
                                        .FirstOrDefaultAsync(cancellationToken);

                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NotFound });
                }

                article.Description = message.Article.Description ?? article.Description;
                article.Body = message.Article.Body ?? article.Body;
                article.Title = message.Article.Title ?? article.Title;
                article.Slug = article.Title.GenerateSlug();

                // list of currently saved article tags for the given article
                var articleTagList = (message.Article.TagList ?? Enumerable.Empty<string>());

                var articleTagsToCreate = GetArticleTagsToCreate(article, articleTagList);
                var articleTagsToDelete = GetArticleTagsToDelete(article, articleTagList);

                if (this._context.ChangeTracker.Entries().First(x => x.Entity == article).State ==
                    EntityState.Modified || articleTagsToCreate.Any() || articleTagsToDelete.Any())
                {
                    article.UpdatedAt = DateTime.UtcNow;
                }

                // add the new article tags
                await this._context.ArticleTags.AddRangeAsync(articleTagsToCreate, cancellationToken);

                // delete the tags that do not exist anymore
                this._context.ArticleTags.RemoveRange(articleTagsToDelete);

                await this._context.SaveChangesAsync(cancellationToken);

                return new ArticleEnvelope(await this._context.Articles.GetAllData()
                                                     .Where(x => x.Slug == article.Slug)
                                                     .FirstOrDefaultAsync(cancellationToken));
            }

            #endregion

            /// <summary>
            /// get the list of Tags to be added
            /// </summary>
            /// <param name="articleTagList"></param>
            /// <returns></returns>
            private async Task<List<Tag>> GetTagsToCreate(IEnumerable<string> articleTagList)
            {
                var tagsToCreate = new List<Tag>();

                foreach (var tag in articleTagList)
                {
                    var t = await this._context.Tags.FindAsync(tag);

                    if (t == null)
                    {
                        t = new Tag() { TagId = tag };
                        tagsToCreate.Add(t);
                    }
                }

                return tagsToCreate;
            }

            /// <summary>
            /// check which article tags need to be added
            /// </summary>
            private static List<ArticleTag> GetArticleTagsToCreate(Article article, IEnumerable<string> articleTagList)
            {
                var articleTagsToCreate = new List<ArticleTag>();

                foreach (var tag in articleTagList)
                {
                    var at = article.ArticleTags.FirstOrDefault(t => t.TagId == tag);

                    if (at == null)
                    {
                        at = new ArticleTag()
                        {
                            Article = article,
                            ArticleId = article.ArticleId,
                            Tag = new Tag() { TagId = tag },
                            TagId = tag
                        };

                        articleTagsToCreate.Add(at);
                    }
                }

                return articleTagsToCreate;
            }

            /// <summary>
            /// check which article tags need to be deleted
            /// </summary>
            private static List<ArticleTag> GetArticleTagsToDelete(Article article, IEnumerable<string> articleTagList)
            {
                var articleTagsToDelete = new List<ArticleTag>();

                foreach (var tag in article.ArticleTags)
                {
                    var at = articleTagList.FirstOrDefault(t => t == tag.TagId);

                    if (at == null)
                    {
                        articleTagsToDelete.Add(tag);
                    }
                }

                return articleTagsToDelete;
            }
        }

        #endregion
    }
}
