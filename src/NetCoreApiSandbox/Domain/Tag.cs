namespace NetCoreApiSandbox.Domain
{
    #region

    using System.Collections.Generic;

    #endregion

    public class Tag
    {
        public string TagId { get; set; }

        public List<ArticleTag> ArticleTags { get; set; }
    }
}
