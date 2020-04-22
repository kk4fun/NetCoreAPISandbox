namespace NetCoreApiSandbox.Features.Comments
{
    #region

    using NetCoreApiSandbox.Domain;

    #endregion

    public class CommentEnvelope
    {
        public CommentEnvelope(Comment comment)
        {
            this.Comment = comment;
        }

        public Comment Comment { get; }
    }
}
