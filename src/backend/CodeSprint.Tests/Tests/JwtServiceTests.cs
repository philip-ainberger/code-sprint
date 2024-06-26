﻿using CodeSprint.Common.Jwt;
using CodeSprint.Common.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace CodeSprint.Tests.Tests;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly Mock<IOptions<JwtOptions>> _optionsMock;

    public JwtServiceTests()
    {
        _optionsMock = new Mock<IOptions<JwtOptions>>();
        _optionsMock.Setup(o => o.Value).Returns(new JwtOptions
        {
            AccessTokensKey = "HMkm9Q8K9ztjhsjbccjfE1BZggliMqY8FpNW5vWRbLZgsveg55Jnv1iPRKTsS62s",
            RefreshTokensKey = "23e7251e6f6ccf687621b8540c3ad4d214429e06aba6dda0bb3c1db28a071388"
        }
        );

        _jwtService = new JwtService(_optionsMock.Object);
    }

    [Fact]
    public void BuildJwt_WithValidUserId_ReturnsValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var jwtInfo = _jwtService.BuildJwt(userId);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwtInfo.TokenValue);

        token.Issuer.Should().Be(_optionsMock.Object.Value.ValidIssuer);
        token.Audiences.First().Should().Be(_optionsMock.Object.Value.ValidAudience);
        jwtInfo.Expiration.Should().BeAfter(DateTime.UtcNow.AddMinutes(14));
        jwtInfo.Expiration.Should().BeBefore(DateTime.UtcNow.AddMinutes(16));
    }
}