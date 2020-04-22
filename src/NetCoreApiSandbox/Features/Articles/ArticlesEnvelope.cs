namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System.Collections.Generic;
    using NetCoreApiSandbox.Domain;

    #endregion

    public class ArticlesEnvelope
    {
        public List<Article> Articles { get; set; }

        public int ArticlesCount { get; set; }
    }
}
