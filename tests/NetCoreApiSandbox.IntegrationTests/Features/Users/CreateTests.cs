namespace NetCoreApiSandbox.IntegrationTests.Features.Users
{
    #region

    using System.Linq;
    using System.Threading.Tasks;
    using NetCoreApiSandbox.Features.Users;
    using NetCoreApiSandbox.Infrastructure.Security;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    #endregion

    public class CreateTests: SliceFixture
    {
        [Fact]
        public async Task Expect_Create_User()
        {
            var command = new Create.Command()
            {
                User = new Create.UserData() { Email = "email", Password = "password", Username = "username" }
            };

            await this.SendAsync(command);

            var created =
                await this.ExecuteDbContextAsync(db => db.Persons.Where(d => d.Email == command.User.Email)
                                                         .SingleOrDefaultAsync());

            Assert.NotNull(created);
            Assert.Equal(created.Hash, new PasswordHasher().Hash("password", created.Salt));
        }
    }
}
