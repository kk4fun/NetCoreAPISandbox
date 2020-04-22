namespace NetCoreApiSandbox.Features.Users
{
    #region

    using AutoMapper;
    using NetCoreApiSandbox.Domain;

    #endregion

    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Person, User>(MemberList.None);
        }
    }
}
