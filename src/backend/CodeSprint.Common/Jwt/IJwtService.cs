namespace CodeSprint.Common.Jwt;

public interface IJwtService
{
    JwtInfo BuildJwt(Guid userId);
    JwtInfo BuildRefreshJwt(Guid userId);
}