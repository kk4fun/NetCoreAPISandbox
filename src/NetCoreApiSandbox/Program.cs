namespace NetCoreApiSandbox
{
    #region

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    #endregion

    public sealed class Program
    {
        public static void Main()
        {
            // read database configuration (database provider + database connection) from environment variables
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var host = new WebHostBuilder().UseConfiguration(config)
                                           .UseKestrel()
                                           .UseUrls("http://+:5000")
                                           .UseStartup<Startup>()
                                           .Build();

            host.Run();
        }

        // EF Core uses this method at design time to access the DbContext
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}
