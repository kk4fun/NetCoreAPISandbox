namespace NetCoreApiSandbox.Features.Profiles
{
    #region

    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;

    #endregion

    public class Details
    {
        #region Nested type: Query

        public class Query: IRequest<ProfileEnvelope>
        {
            public string Username { get; set; }
        }

        #endregion

        #region Nested type: QueryHandler

        public sealed class QueryHandler: IRequestHandler<Query, ProfileEnvelope>
        {
            private readonly IProfileReader _profileReader;

            public QueryHandler(IProfileReader profileReader)
            {
                this._profileReader = profileReader;
            }

            #region IRequestHandler<Query,ProfileEnvelope> Members

            public async Task<ProfileEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                return await this._profileReader.ReadProfile(message.Username);
            }

            #endregion
        }

        #endregion

        #region Nested type: QueryValidator

        public sealed class QueryValidator: AbstractValidator<Query>
        {
            public QueryValidator()
            {
                this.RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        #endregion
    }
}
