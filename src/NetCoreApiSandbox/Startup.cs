namespace NetCoreApiSandbox
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using NetCoreApiSandbox.Features.Profiles;
    using NetCoreApiSandbox.Infrastructure;
    using NetCoreApiSandbox.Infrastructure.Errors;
    using NetCoreApiSandbox.Infrastructure.Security;
    using Newtonsoft.Json;

    #endregion

    /// <summary>
    ///     Main configuration class
    /// </summary>
    public class Startup
    {
        private const string DefaultDatabaseConnectionString = "Filename=netcore-api-sandbox.db";

        private const string DefaultDatabaseProvider = "sqlite";

        // private const string DefaultDatabaseConnectionString = @"Server=(localdb)\mssqllocaldb;Database=sandbox;Trusted_Connection=True;";
        // private const string DefaultDatabaseProvider = "sqlserver";

        private readonly IConfiguration _config;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="config">Default configuration.</param>
        public Startup(IConfiguration config)
        {
            this._config = config;
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container
        ///     or more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <exception cref="Exception">Throws exception if Database provider is unknown.</exception>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));

            // take the connection string from the environment variable or use hard-coded database name
            var connectionString = this._config.GetValue<string>("NetCoreApiSandbox_ConnectionString") ??
                                   DefaultDatabaseConnectionString;

            // take the database provider from the environment variable or use hard-coded database provider
            var databaseProvider = this._config.GetValue<string>("NetCoreApiSandbox_DatabaseProvider");

            if (string.IsNullOrWhiteSpace(databaseProvider))
            {
                databaseProvider = DefaultDatabaseProvider;
            }

            services.AddDbContext<NetCoreSandboxApiContext>(options =>
            {
                if (databaseProvider.ToLower(CultureInfo.CurrentCulture)
                                    .Trim()
                                    .Equals("sqlite", StringComparison.CurrentCulture))
                {
                    options.UseSqlite(connectionString);
                }
                else if (databaseProvider.ToLower(CultureInfo.CurrentCulture)
                                         .Trim()
                                         .Equals("sqlserver", StringComparison.CurrentCulture))

                {
                    // only works in windows container
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    throw new Exception("Database provider unknown. Please check configuration");
                }
            });

            services.AddLocalization(x => x.ResourcesPath = "Resources");

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("Bearer",
                                        new OpenApiSecurityScheme
                                        {
                                            In = ParameterLocation.Header,
                                            Description = "Please insert JWT with Bearer into field",
                                            Name = "Authorization",
                                            Type = SecuritySchemeType.ApiKey,
                                            BearerFormat = "JWT"
                                        });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Net core API sandbox", Version = "v1" });
                x.CustomSchemaIds(y => y.FullName);
                x.DocInclusionPredicate((version, apiDescription) => true);
                x.TagActionsBy(y => new List<string> { y.GroupName });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            services.AddCors();

            services.AddMvc(opt =>
                     {
                         opt.Conventions.Add(new GroupByApiRootConvention());
                         opt.Filters.Add(typeof(ValidatorActionFilter));
                         opt.EnableEndpointRouting = false;
                     })
                    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling =
                                                      ReferenceLoopHandling.Ignore)
                    .AddJsonOptions(opt => { opt.JsonSerializerOptions.IgnoreNullValues = true; })
                    .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });

            services.AddAutoMapper(this.GetType().Assembly);

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddScoped<IProfileReader, ProfileReader>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddJwt();
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilogLogging();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            // Enable middleware to serve swagger-ui assets(HTML, JS, CSS etc.)
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "Net Core API sandbox API V1"); });

            app.ApplicationServices.GetRequiredService<NetCoreSandboxApiContext>().Database.EnsureCreated();
        }
    }
}
