namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using System.Collections.Generic;
    using NetCoreApiSandbox.Domain;

    #endregion

    public class CommentsEnvelope
    {
        public CommentsEnvelope(List<Comment> comments)
        {
            this.Comments = comments;
        }

        public List<Comment> Comments { get; }
    }
}
