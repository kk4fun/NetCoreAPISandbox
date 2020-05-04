namespace NetCoreApiSandbox.Features.Users
{
    public class UserLoginDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class UserDTO: UserLoginDTO
    {
        public string Username { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }
    }
}
