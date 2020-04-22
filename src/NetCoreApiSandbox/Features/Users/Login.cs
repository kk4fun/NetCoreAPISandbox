namespace NetCoreApiSandbox.Features.Users
{
    #region

    using System.Linq;
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

    public class Login
    {
        #region Nested type: Command

        public class Command: IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        #endregion

        #region Nested type: CommandValidator

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
            }
        }

        #endregion

        #region Nested type: Handler

        public class Handler: IRequestHandler<Command, UserEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly IMapper _mapper;
            private readonly IPasswordHasher _passwordHasher;

            public Handler(
                NetCoreSandboxApiContext context,
                IPasswordHasher passwordHasher,
                IJwtTokenGenerator jwtTokenGenerator,
                IMapper mapper)
            {
                this._context = context;
                this._passwordHasher = passwordHasher;
                this._jwtTokenGenerator = jwtTokenGenerator;
                this._mapper = mapper;
            }

            #region IRequestHandler<Command,UserEnvelope> Members

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var person = await this._context.Persons.Where(x => x.Email == message.User.Email)
                                       .SingleOrDefaultAsync(cancellationToken);

                if (person == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Invalid email / password." });
                }

                if (!person.Hash.SequenceEqual(this._passwordHasher.Hash(message.User.Password, person.Salt.ToArray())))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Invalid email / password." });
                }

                var user = this._mapper.Map<Person, User>(person);
                user.Token = await this._jwtTokenGenerator.CreateToken(person.Username);

                return new UserEnvelope(user);
            }

            #endregion
        }

        #endregion

        #region Nested type: UserData

        public class UserData
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

        #endregion

        #region Nested type: UserDataValidator

        public class UserDataValidator: AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                this.RuleFor(x => x.Email).NotNull().NotEmpty();
                this.RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        #endregion
    }
}
