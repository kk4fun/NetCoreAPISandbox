namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using NetCoreApiSandbox.Domain;

    #endregion

    public class ArticleEnvelope
    {
        public ArticleEnvelope(Article article)
        {
            this.Article = article;
        }

        public Article Article { get; }
    }
}
