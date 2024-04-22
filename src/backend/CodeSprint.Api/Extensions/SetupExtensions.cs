using CodeSprint.Common.Jwt;
using CodeSprint.Common.Options;
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

    public static void AddMongoClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(prov =>
        {
            var config = prov.GetRequiredService<IOptions<MongoOptions>>().Value;

            var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
            settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

            return new MongoClient(settings);
        });
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