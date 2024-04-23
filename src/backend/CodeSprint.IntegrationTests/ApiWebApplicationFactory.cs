using CodeSprint.Api.Services;
using CodeSprint.Common.Dtos;
using CodeSprint.Tests.Common;
using CodeSprint.Common.Extensions;
using CodeSprint.Common.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace CodeSprint.IntegrationTests;

public class ApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly MongoDbRunner _mongoRunner;

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
                Jwt = new JwtOptions() 
                { 
                    AccessTokensKey = "HMkm9Q8K9ztjhsjbccjfE1BZggliMqY8FpNW5vWRbLZgsveg55Jnv1iPRKTsS62s",
                    RefreshTokensKey = "23e7251e6f6ccf687621b8540c3ad4d214429e06aba6dda0bb3c1db28a071388"
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(config);
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

    protected override void Dispose(bool disposing)
    {
        _mongoRunner.Dispose();
        base.Dispose(disposing);
    }

    private Mock<IGitHubOAuthService> GetGitHubOAuthServiceMock()
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
