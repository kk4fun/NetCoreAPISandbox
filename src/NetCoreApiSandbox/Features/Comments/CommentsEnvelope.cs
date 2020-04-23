namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using System.Collections.Generic;
    using NetCoreApiSandbox.Domain;

    #endregion

    public class CommentsEnvelope
    {
        public CommentsEnvelope(ICollection<Comment> comments)
        {
            this.Comments = comments;
        }

        public ICollection<Comment> Comments { get; }
    }
}
