using CodeSprint.Api.Repositories;
using CodeSprint.Common.Jwt;
using CodeSprint.Common.Options;
using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

namespace CodeSprint.Api.Extensions;

public static class SetupExtensions
{
    public static void AddCustomOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection(MongoOptions.Section));
        services.Configure<GithubOAuthOptions>(configuration.GetSection(GithubOAuthOptions.Section));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Section));
    }
    public static void AddCustomJwtAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IJwtService, JwtService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration
                    .GetSection("Jwt")
                    .Get<JwtOptions>();

                if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.Key))
                    throw new ArgumentException("");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidIssuer = jwtOptions.ValidIssuer,
                    ValidAudience = jwtOptions.ValidAudience
                };
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
            options.AddPolicy("AllowAngularDev",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .AllowAnyHeader();
                });
        });
    }
}