namespace NetCoreApiSandbox.Features.Users
{
    #region

    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Security;

    #endregion

    public class Edit
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
                this.RuleFor(x => x.User).NotNull();
            }
        }

        #endregion

        #region Nested type: Handler

        public class Handler: IRequestHandler<Command, UserEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IMapper _mapper;
            private readonly IPasswordHasher _passwordHasher;

            public Handler(
                NetCoreSandboxApiContext context,
                IPasswordHasher passwordHasher,
                ICurrentUserAccessor currentUserAccessor,
                IMapper mapper)
            {
                this._context = context;
                this._passwordHasher = passwordHasher;
                this._currentUserAccessor = currentUserAccessor;
                this._mapper = mapper;
            }

            #region IRequestHandler<Command,UserEnvelope> Members

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var currentUsername = this._currentUserAccessor.GetCurrentUsername();

                var person = await this._context.Persons.Where(x => x.Username == currentUsername)
                                       .FirstOrDefaultAsync(cancellationToken);

                person.Username = message.User.Username ?? person.Username;
                person.Email = message.User.Email ?? person.Email;
                person.Bio = message.User.Bio ?? person.Bio;
                person.Image = message.User.Image ?? person.Image;

                if (!string.IsNullOrWhiteSpace(message.User.Password))
                {
                    var salt = Guid.NewGuid().ToByteArray();
                    person.Hash = this._passwordHasher.Hash(message.User.Password, salt);
                    person.Salt = salt;
                }

                await this._context.SaveChangesAsync(cancellationToken);

                return new UserEnvelope(this._mapper.Map<Person, User>(person));
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

            public string Bio { get; set; }

            public string Image { get; set; }
        }

        #endregion
    }
}
