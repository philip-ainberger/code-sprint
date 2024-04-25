using CodeSprint.Api.Repositories;
using CodeSprint.Common;
using CodeSprint.Common.Jwt;
using CodeSprint.Common.Options;
using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CodeSprint.Api.Extensions;

public static class SetupExtensions
{
    public static void AddCustomOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        var customSection = configuration.GetSection("Custom");

        services.Configure<MongoOptions>(customSection.GetSection(MongoOptions.Section));
        services.Configure<GithubOAuthOptions>(customSection.GetSection(GithubOAuthOptions.Section));
        services.Configure<JwtOptions>(customSection.GetSection(JwtOptions.Section));
        services.Configure<ApplicationOptions>(customSection.GetSection(ApplicationOptions.Section));
    }

    public static void AddCustomJwtAuthentication(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = services
                    .BuildServiceProvider()
                    .GetRequiredService<IOptions<JwtOptions>>()
                    .Value!;

                options.TokenValidationParameters = Defaults.GetDefaultTokenValidationParameters(
                    jwtOptions.AccessTokensKey,
                    jwtOptions.ValidIssuer,
                    jwtOptions.ValidAudience);
            });
    }

    public static IServiceCollection AddMongoDbAccess(this IServiceCollection services)
    {
        return services
            .AddSingleton<IMongoClient>(prov =>
            {
                var config = prov.GetRequiredService<IOptions<MongoOptions>>().Value;

                var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
                settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

                return new MongoClient(settings);
            })
            .AddSingleton<IMongoDatabase>(prov =>
            {
                var client = prov.GetRequiredService<IMongoClient>();
                var config = prov.GetRequiredService<IOptions<MongoOptions>>().Value;
                return client.GetDatabase(config.DatabaseName);
            });
    }

    public static void AddCustomMongoDbCollections(this IServiceCollection services)
    {
        services
            .AddCustomMongoDbCollections<RefreshToken, IRefreshTokenRepository, RefreshTokenRepository>("refresh_tokens")
            .AddCustomMongoDbCollections<Core.Models.Tag, ITaggingRepository, TaggingRepository>("tags")
            .AddCustomMongoDbCollections<Sprint, ICodingRepository, CodingRepository>("sprints")
            .AddCustomMongoDbCollections<SprintActivity, ISprintActivityRepository, SprintActivityRepository>("sprint_activities")
            .AddCustomMongoDbCollections<User, IUserRepository, UserRepository>("users");
    }

    public static IServiceCollection AddCustomMongoDbCollections<TModel, TRepositoryInterface, TRepositoryImpl>(this IServiceCollection services, string collectionName)
        where TRepositoryImpl : class, TRepositoryInterface
        where TRepositoryInterface : class
    {
        return services
            .AddScoped(prov =>
            {
                var db = prov.GetRequiredService<IMongoDatabase>();
                return db.GetCollection<TModel>(collectionName);
            })
            .AddScoped<TRepositoryInterface, TRepositoryImpl>();
    }

    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            var applicationOptions = services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<ApplicationOptions>>()
                .Value!;

            options.AddPolicy(Environments.Production,
                builder =>
                {
                    builder.WithOrigins(applicationOptions.HostedClientUrl);
                });

            options.AddPolicy(Environments.Development,
                builder =>
                {
                    builder.WithOrigins(applicationOptions.HostedClientUrl)
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .AllowAnyHeader();
                });
        });
    }
}