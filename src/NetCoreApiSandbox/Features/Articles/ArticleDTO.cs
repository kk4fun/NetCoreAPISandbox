namespace NetCoreApiSandbox.Features.Articles
{
    #region

    using System.Collections.Generic;

    #endregion

    public class ArticleDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public IEnumerable<string> TagList { get; set; }
    }
}
