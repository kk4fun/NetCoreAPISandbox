namespace NetCoreApiSandbox.Features.Users
{
    #region

    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;
    using NetCoreApiSandbox.Infrastructure.Security;

    #endregion

    public class Details
    {
        #region Nested type: Query

        public class Query: IRequest<UserEnvelope>
        {
            public string Username { get; set; }
        }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Query, UserEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly IMapper _mapper;

            public QueryHandler(NetCoreSandboxApiContext context, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
            {
                this._context = context;
                this._jwtTokenGenerator = jwtTokenGenerator;
                this._mapper = mapper;
            }

            #region IRequestHandler<Query,UserEnvelope> Members

            public async Task<UserEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var person = await this._context.Persons.AsNoTracking()
                                       .FirstOrDefaultAsync(x => x.Username == message.Username, cancellationToken);

                if (person == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = Constants.NotFound });
                }

                var user = this._mapper.Map<Person, User>(person);
                user.Token = await this._jwtTokenGenerator.CreateToken(person.Username);

                return new UserEnvelope(user);
            }

            #endregion
        }

        #endregion

        #region Nested type: QueryValidator

        public class QueryValidator: AbstractValidator<Query>
        {
            public QueryValidator()
            {
                this.RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        #endregion
    }
}
