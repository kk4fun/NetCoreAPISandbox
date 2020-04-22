namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;

    #endregion

    public static class ArticleExtensions
    {
        public static IQueryable<Article> GetAllData(this DbSet<Article> articles)
        {
            return articles.Include(x => x.Author)
                           .Include(x => x.ArticleFavorites)
                           .Include(x => x.ArticleTags)
                           .AsNoTracking();
        }
    }
}
