namespace NetCoreApiSandbox.IntegrationTests
{
    #region

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using NetCoreApiSandbox.Infrastructure;

    #endregion

    public class SliceFixture: IDisposable
    {
        private static readonly IConfiguration Config;
        private readonly string _dbName = string.Concat(Guid.NewGuid().ToString(), ".db");
        private readonly ServiceProvider _provider;

        private readonly IServiceScopeFactory _scopeFactory;

        static SliceFixture()
        {
            Config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        }

        protected SliceFixture()
        {
            var mockWebhostEnv = new Mock<IWebHostEnvironment>();
            var startup = new Startup(Config, mockWebhostEnv.Object);
            var services = new ServiceCollection();

            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase(this._dbName);
            services.AddSingleton(new NetCoreSandboxApiContext(builder.Options));

            startup.ConfigureServices(services);

            this._provider = services.BuildServiceProvider();

            this.GetDbContext().Database.EnsureCreated();
            this._scopeFactory = this._provider.GetService<IServiceScopeFactory>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            File.Delete(this._dbName);
        }

        #endregion

        public NetCoreSandboxApiContext GetDbContext()
        {
            return this._provider.GetRequiredService<NetCoreSandboxApiContext>();
        }

        private async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = this._scopeFactory.CreateScope();

            await action(scope.ServiceProvider);
        }

        private async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = this._scopeFactory.CreateScope();

            return await action(scope.ServiceProvider);
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return this.ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task SendAsync(IRequest request)
        {
            return this.ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task ExecuteDbContextAsync(Func<NetCoreSandboxApiContext, Task> action)
        {
            return this.ExecuteScopeAsync(sp => action(sp.GetService<NetCoreSandboxApiContext>()));
        }

        public Task<T> ExecuteDbContextAsync<T>(Func<NetCoreSandboxApiContext, Task<T>> action)
        {
            return this.ExecuteScopeAsync(sp => action(sp.GetService<NetCoreSandboxApiContext>()));
        }

        protected Task InsertAsync(params object[] entities)
        {
            return this.ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Add(entity);
                }

                return db.SaveChangesAsync();
            });
        }
    }
}
