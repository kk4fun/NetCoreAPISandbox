namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Infrastructure;

    #endregion

    public static class List
    {
        #region Nested type: Query

        public class Query: IRequest<ArticlesEnvelope>
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="Query" /> class.
            /// </summary>
            /// <param name="tag">Article tag</param>
            /// <param name="author"></param>
            /// <param name="favorited"></param>
            /// <param name="limit"></param>
            /// <param name="offset"></param>
            public Query(string tag, string author, string favorited, int? limit, int? offset)
            {
                this.Tag = tag;
                this.Author = author;
                this.FavoritedUsername = favorited;
                this.Limit = limit;
                this.Offset = offset;
            }

            /// <summary>
            ///     Gets article tag
            /// </summary>
            public string Tag { get; }

            public string Author { get; }

            public string FavoritedUsername { get; }

            public int? Limit { get; }

            public int? Offset { get; }

            public bool IsFeed { get; set; }
        }

        #endregion

        #region Nested type: QueryHandler

        /// <summary>
        ///     MediatR query handler
        /// </summary>
        private class QueryHandler: IRequestHandler<Query, ArticlesEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            /// <summary>
            ///     Initializes a new instance of the <see cref="QueryHandler" /> class.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="currentUserAccessor"></param>
            public QueryHandler(NetCoreSandboxApiContext context, ICurrentUserAccessor currentUserAccessor)
            {
                this._context = context;
                this._currentUserAccessor = currentUserAccessor;
            }

            #region IRequestHandler<Query,ArticlesEnvelope> Members

            public async Task<ArticlesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var queryable = this._context.Articles.GetAllData();

                if (message.IsFeed && this._currentUserAccessor.GetCurrentUsername() != null)
                {
                    var currentUser = await this._context.Users.Include(x => x.Following)
                                                .FirstOrDefaultAsync(x => x.Username == this._currentUserAccessor
                                                                                            .GetCurrentUsername(),
                                                                     cancellationToken);

                    queryable =
                        queryable.Where(x => currentUser.Following.Select(y => y.TargetId).Contains(x.Author.Id));
                }

                if (!string.IsNullOrWhiteSpace(message.Tag))
                {
                    var tag = await this._context.ArticleTags.FirstOrDefaultAsync(x => x.TagId == message.Tag,
                                                                                  cancellationToken);

                    if (tag != null)
                    {
                        queryable = queryable.Where(x => x.ArticleTags.Select(y => y.TagId).Contains(tag.TagId));
                    }
                    else
                    {
                        return new ArticlesEnvelope();
                    }
                }

                if (!string.IsNullOrWhiteSpace(message.Author))
                {
                    var author =
                        await this._context.Users.FirstOrDefaultAsync(x => x.Username == message.Author,
                                                                      cancellationToken);

                    if (author != null)
                    {
                        queryable = queryable.Where(x => x.Author == author);
                    }
                    else
                    {
                        return new ArticlesEnvelope();
                    }
                }

                if (!string.IsNullOrWhiteSpace(message.FavoritedUsername))
                {
                    var author =
                        await this._context.Users.FirstOrDefaultAsync(x => x.Username == message.FavoritedUsername,
                                                                      cancellationToken);

                    if (author != null)
                    {
                        queryable = queryable.Where(x => x.ArticleFavorites.Any(y => y.UserId == author.Id));
                    }
                    else
                    {
                        return new ArticlesEnvelope();
                    }
                }

                var articles = await queryable.OrderByDescending(x => x.CreatedAt)
                                              .Skip(message.Offset ?? 0)
                                              .Take(message.Limit ?? 20)
                                              .AsNoTracking()
                                              .ToListAsync(cancellationToken);

                return new ArticlesEnvelope { Articles = articles, ArticlesCount = queryable.Count() };
            }

            #endregion
        }

        #endregion
    }
}
