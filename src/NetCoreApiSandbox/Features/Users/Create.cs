namespace NetCoreApiSandbox.Features.Users
{
    #region

    using System;
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

    public sealed class Create
    {
        #region Nested type: Command

        public class Command: IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        #endregion

        #region Nested type: CommandValidator

        public sealed class CommandValidator: AbstractValidator<Command>
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
                if (await this._context.Users.Where(x => x.Username == message.User.Username)
                              .AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = Constants.InUse });
                }

                if (await this._context.Users.FirstOrDefaultAsync(x => x.Email == message.User.Email,
                                                                  cancellationToken) != null)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = Constants.InUse });
                }

                var salt = Guid.NewGuid().ToByteArray();

                var person = new User
                {
                    Username = message.User.Username,
                    Email = message.User.Email,
                    Hash = this._passwordHasher.Hash(message.User.Password, salt),
                    Salt = salt
                };

                this._context.Users.Add(person);
                await this._context.SaveChangesAsync(cancellationToken);
                var user = this._mapper.Map<User, UserDTO>(person);
                user.Token = await this._jwtTokenGenerator.CreateToken(person.Username);

                return new UserEnvelope(user);
            }

            #endregion
        }

        #endregion

        #region Nested type: UserData

        public class UserData
        {
            public string Username { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }
        }

        #endregion

        #region Nested type: UserDataValidator

        public class UserDataValidator: AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                this.RuleFor(x => x.Username).NotNull().NotEmpty();
                this.RuleFor(x => x.Email).NotNull().NotEmpty();
                this.RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        #endregion
    }
}
