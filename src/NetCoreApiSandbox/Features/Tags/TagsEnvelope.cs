namespace NetCoreApiSandbox.Features.Tags
{
    #region

    using System.Collections.Generic;

    #endregion

    public class TagsEnvelope
    {
        public IEnumerable<string> Tags { get; set; }
    }
}
