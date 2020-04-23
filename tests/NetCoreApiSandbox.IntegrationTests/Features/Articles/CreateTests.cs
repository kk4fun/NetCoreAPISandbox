namespace NetCoreApiSandbox.IntegrationTests.Features.Articles
{
    #region

    using System.Linq;
    using System.Threading.Tasks;
    using NetCoreApiSandbox.Features.Articles;
    using Xunit;

    #endregion

    public class CreateTests: SliceFixture
    {
        [Fact]
        public async Task Expect_Create_Article()
        {
            var command = new Create.Command
            {
                Article = new ArticleDTO
                {
                    Title = "Test article dsergiu77",
                    Description = "Description of the test article",
                    Body = "Body of the test article",
                    TagList = new[] { "tag1", "tag2" }
                }
            };

            var article = await ArticleHelpers.CreateArticle(this, command);

            Assert.NotNull(article);
            Assert.Equal(article.Title, command.Article.Title);
            Assert.Equal(article.TagList.Count(), command.Article.TagList.Count());
        }
    }
}
