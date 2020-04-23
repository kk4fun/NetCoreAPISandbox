namespace NetCoreApiSandbox.IntegrationTests.Features.Articles
{
    #region

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NetCoreApiSandbox.Features.Articles;
    using Xunit;

    #endregion

    public class EditTests: SliceFixture
    {
        [Fact]
        public async Task Expect_Edit_Article()
        {
            var createCommand = new Create.Command
            {
                Article = new ArticleDTO
                {
                    Title = "Test article dsergiu77",
                    Description = "Description of the test article",
                    Body = "Body of the test article",
                    TagList = new[] { "tag1", "tag2" }
                }
            };

            var createdArticle = await ArticleHelpers.CreateArticle(this, createCommand);

            var command = new Edit.Command
            {
                Article = new ArticleDTO
                {
                    Title = "Updated " + createdArticle.Title,
                    Description = "Updated" + createdArticle.Description,
                    Body = "Updated" + createdArticle.Body
                },
                Slug = createdArticle.Slug
            };

            // remove the first tag and add a new tag
            command.Article.TagList = new[] { createdArticle.TagList.ToArray()[1], "tag3" };

            var dbContext = this.GetDbContext();

            var articleEditHandler = new Edit.Handler(dbContext);
            var edited = await articleEditHandler.Handle(command, new CancellationToken());

            Assert.NotNull(edited);
            Assert.Equal(edited.Article.Title, command.Article.Title);
            var arrayTags = edited.Article.TagList.ToArray();
            Assert.Equal(arrayTags.Count(), command.Article.TagList.Count());

            // use assert Contains because we do not know the order in which the tags are saved/retrieved
            Assert.Contains(arrayTags[0], command.Article.TagList);
            Assert.Contains(arrayTags[1], command.Article.TagList);
        }
    }
}
