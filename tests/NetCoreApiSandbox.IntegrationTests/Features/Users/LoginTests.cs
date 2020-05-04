namespace NetCoreApiSandbox.IntegrationTests.Features.Users
{
    #region

    using System;
    using System.Threading.Tasks;
    using NetCoreApiSandbox.Domain;
    using NetCoreApiSandbox.Features.Users;
    using NetCoreApiSandbox.Infrastructure.Security;
    using Xunit;

    #endregion

    public class LoginTests: SliceFixture
    {
        [Fact]
        public async Task Expect_Login()
        {
            var salt = Guid.NewGuid().ToByteArray();

            var person = new Person
            {
                Username = "username",
                Email = "email",
                Hash = new PasswordHasher().Hash("password", salt),
                Salt = salt
            };

            await this.InsertAsync(person);

            var command = new Login.Command { User = new Login.UserData { Email = "email", Password = "password" } };

            var user = await this.SendAsync(command);

            Assert.NotNull(user?.User);
            Assert.Equal(user.User.Email, command.User.Email);
            Assert.Equal("username", user.User.Username);
            Assert.NotNull(user.User.Token);
        }
    }
}
