namespace NetCoreApiSandbox.Features.Profiles
{
    #region

    using AutoMapper;
    using NetCoreApiSandbox.Domain;

    #endregion

    public class MappingProfile: AutoMapper.Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Person, Profile>(MemberList.None);
        }
    }
}
