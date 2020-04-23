namespace NetCoreApiSandbox.Features.Followers
{
    #region

    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Features.Profiles;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public class Delete
    {
        #region Nested type: Command

        public class Command: IRequest<ProfileEnvelope>
        {
            public Command(string username)
            {
                this.Username = username;
            }

            public string Username { get; }
        }

        #endregion

        #region Nested type: CommandValidator

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                this.RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        #endregion

        #region Nested type: QueryHandler

        public class QueryHandler: IRequestHandler<Command, ProfileEnvelope>
        {
            private readonly NetCoreSandboxApiContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IProfileReader _profileReader;

            public QueryHandler(
                NetCoreSandboxApiContext context,
                ICurrentUserAccessor currentUserAccessor,
                IProfileReader profileReader)
            {
                this._context = context;
                this._currentUserAccessor = currentUserAccessor;
                this._profileReader = profileReader;
            }

            #region IRequestHandler<Command,ProfileEnvelope> Members

            public async Task<ProfileEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var target =
                    await this._context.Persons.FirstOrDefaultAsync(x => x.Username == message.Username,
                                                                    cancellationToken);

                if (target == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = Constants.NotFound });
                }

                var observer =
                    await this._context.Persons.FirstOrDefaultAsync(x => x.Username ==
                                                                         this._currentUserAccessor.GetCurrentUsername(),
                                                                    cancellationToken);

                var followedPeople =
                    await this._context.FollowedPeople.FirstOrDefaultAsync(x => x.ObserverId == observer.Id &&
                                                                                x.TargetId == target.Id,
                                                                           cancellationToken);

                if (followedPeople != null)
                {
                    this._context.FollowedPeople.Remove(followedPeople);
                    await this._context.SaveChangesAsync(cancellationToken);
                }

                return await this._profileReader.ReadProfile(message.Username);
            }

            #endregion
        }

        #endregion
    }
}
