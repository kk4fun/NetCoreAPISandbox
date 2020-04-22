namespace NetCoreApiSandbox.IntegrationTests.Features.Articles
{
    #region

    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Features.Articles;
    using NetCoreApiSandbox.IntegrationTests.Features.Comments;
    using NetCoreApiSandbox.IntegrationTests.Features.Users;
    using Xunit;
    using comments = NetCoreApiSandbox.Features.Comments;

    #endregion

    public class DeleteTests: SliceFixture
    {
        [Fact]
        public async Task Expect_Delete_Article()
        {
            var createCmd = new Create.Command()
            {
                Article = new Create.ArticleData()
                {
                    Title = "Test article dsergiu77",
                    Description = "Description of the test article",
                    Body = "Body of the test article"
                }
            };

            var article = await ArticleHelpers.CreateArticle(this, createCmd);
            var slug = article.Slug;

            var deleteCmd = new Delete.Command(slug);

            var dbContext = this.GetDbContext();

            var articleDeleteHandler = new Delete.QueryHandler(dbContext);
            await articleDeleteHandler.Handle(deleteCmd, default);

            var dbArticle =
                await this.ExecuteDbContextAsync(db => db.Articles.Where(d => d.Slug == deleteCmd.Slug)
                                                         .SingleOrDefaultAsync());

            Assert.Null(dbArticle);
        }

        [Fact]
        public async Task Expect_Delete_Article_With_Comments()
        {
            var createArticleCmd = new Create.Command()
            {
                Article = new Create.ArticleData()
                {
                    Title = "Test article dsergiu77",
                    Description = "Description of the test article",
                    Body = "Body of the test article"
                }
            };

            var article = await ArticleHelpers.CreateArticle(this, createArticleCmd);

            var dbArticle = await this.ExecuteDbContextAsync(db => db.Articles.Include(a => a.ArticleTags)
                                                                     .Where(d => d.Slug == article.Slug)
                                                                     .SingleOrDefaultAsync());

            var articleId = dbArticle.ArticleId;
            var slug = dbArticle.Slug;

            // create article comment
            var createCommentCmd = new comments.Create.Command()
            {
                Comment = new comments.Create.CommentData() { Body = "article comment" }, Slug = slug
            };

            var comment = await CommentHelpers.CreateComment(this, createCommentCmd, UserHelpers.DefaultUserName);

            // delete article with comment
            var deleteCmd = new Delete.Command(slug);

            var dbContext = this.GetDbContext();

            var articleDeleteHandler = new Delete.QueryHandler(dbContext);
            await articleDeleteHandler.Handle(deleteCmd, default);

            var deleted =
                await this.ExecuteDbContextAsync(db => db.Articles.Where(d => d.Slug == deleteCmd.Slug)
                                                         .SingleOrDefaultAsync());

            Assert.Null(deleted);
        }

        /// <summary>
        /// Expects article to be deleted with tags
        /// </summary>
        /// <returns>Async Task object.</returns>
        [Fact]
        public async Task Expect_Delete_Article_With_Tags()
        {
            var createCmd = new Create.Command()
            {
                Article = new Create.ArticleData()
                {
                    Title = "Test article dsergiu77",
                    Description = "Description of the test article",
                    Body = "Body of the test article",
                    TagList = new string[] { "tag1", "tag2" }
                }
            };

            var article = await ArticleHelpers.CreateArticle(this, createCmd);

            var dbArticleWithTags =
                await this.ExecuteDbContextAsync(db => db.Articles.Include(a => a.ArticleTags)
                                                         .Where(d => d.Slug == article.Slug)
                                                         .SingleOrDefaultAsync());

            var deleteCmd = new Delete.Command(article.Slug);

            var dbContext = this.GetDbContext();

            var articleDeleteHandler = new Delete.QueryHandler(dbContext);
            await articleDeleteHandler.Handle(deleteCmd, default);

            var dbArticle =
                await this.ExecuteDbContextAsync(db => db.Articles.Where(d => d.Slug == deleteCmd.Slug)
                                                         .SingleOrDefaultAsync());

            Assert.Null(dbArticle);
        }
    }
}
