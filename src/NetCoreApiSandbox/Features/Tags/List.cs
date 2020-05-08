namespace NetCoreApiSandbox.Features.Tags
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Infrastructure;

    #endregion

    public static class List
    {
        #region Nested type: Query

        public class Query: IRequest<TagsEnvelope> { }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Query, TagsEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;

            public QueryHandler(NetCoreSandboxApiContext context)
            {
                this._context = context;
            }

            #region IRequestHandler<Query,TagsEnvelope> Members

            public async Task<TagsEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                IEnumerable<Tag> tags =
                    await this._context.Tags.OrderBy(x => x.Id).AsNoTracking().ToListAsync(cancellationToken);

                return new TagsEnvelope { Tags = tags.Select(x => x.Id).ToList() };
            }

            #endregion
        }

        #endregion
    }
}
