using CodeSprint.Api.Services;
using CodeSprint.Common.Dtos;
using CodeSprint.Tests.Common;
using CodeSprint.Common.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Mongo2Go;
using Moq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CodeSprint.IntegrationTests;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly JwtOptions _defaultOptions = new()
    {
        AccessTokensKey = "HMkm9Q8K9ztjhsjbccjfE1BZggliMqY8FpNW5vWRbLZgsveg55Jnv1iPRKTsS62s",
        RefreshTokensKey = "23e7251e6f6ccf687621b8540c3ad4d214429e06aba6dda0bb3c1db28a071388"
    };

    public string MongoConnectionString => _mongoRunner.ConnectionString;
    
    public ApiWebApplicationFactory()
    {
        _mongoRunner = MongoDbRunner.Start();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.Sources.Clear();

            var config = new
            {
                MongoDb = new MongoOptions() { ConnectionString = _mongoRunner.ConnectionString, DatabaseName = "default" },
                Jwt = _defaultOptions
            };

            var json = JsonSerializer.Serialize(config);
            var stream = new MemoryStream();
            var buffer = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;

            configBuilder.AddJsonStream(stream);
        });

        builder.ConfigureServices((context, services) =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IGitHubOAuthService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.Replace<IGitHubOAuthService>(GetGitHubOAuthServiceMock().Object, ServiceLifetime.Scoped);
        });
    }

    internal string GetJwtToken()
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_defaultOptions.AccessTokensKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claim = new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString());

        var expiration = DateTime.UtcNow.AddMinutes(2);
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>() { claim }),
            Expires = expiration,
            SigningCredentials = credentials,
            Issuer = "code-sprint",
            Audience = "code-sprint-api"
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtTokenHandler.CreateToken(securityTokenDescriptor);
        return jwtTokenHandler.WriteToken(securityToken);
    }

    protected override void Dispose(bool disposing)
    {
        _mongoRunner.Dispose();
        base.Dispose(disposing);
    }

    private static Mock<IGitHubOAuthService> GetGitHubOAuthServiceMock()
    {
        var mock = new Mock<IGitHubOAuthService>();

        mock.Setup(x => x.GetBearerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync("mockBearerToken");

        mock.Setup(x => x.GetUserInfoAsync(It.IsAny<string>()))
          .ReturnsAsync(new GitHubUserInfoDto()
          {
              Id = 1,
              Name = "TestUser",
              NodeId = "1",
              AvatarUrl = "",
              Email = "",
              Login = "username"
          });

        return mock;
    }
}
