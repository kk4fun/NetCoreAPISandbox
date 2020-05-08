namespace NetCoreApiSandbox.Features.Profiles
{
    #region

    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;

    #endregion

    public class ProfileReader: IProfileReader
    {
        private readonly NetCoreSandboxApiContext _context;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMapper _mapper;

        public ProfileReader(NetCoreSandboxApiContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
        {
            this._context = context;
            this._currentUserAccessor = currentUserAccessor;
            this._mapper = mapper;
        }

        #region IProfileReader Members

        public async Task<ProfileEnvelope> ReadProfile(string username)
        {
            var currentUserName = this._currentUserAccessor.GetCurrentUsername();

            var person = await this._context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

            if (person == null)
            {
                throw new RestException(HttpStatusCode.NotFound, new { User = Constants.NotFound });
            }

            var profile = this._mapper.Map<User, Profile>(person);

            if (currentUserName != null)
            {
                var currentPerson = await this._context.Users.Include(x => x.Following)
                                              .Include(x => x.Followers)
                                              .FirstOrDefaultAsync(x => x.Username == currentUserName);

                if (currentPerson.Followers.Any(x => x.TargetId == person.Id))
                {
                    profile.IsFollowed = true;
                }
            }

            return new ProfileEnvelope(profile);
        }

        #endregion
    }
}
