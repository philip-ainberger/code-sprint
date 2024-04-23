using System.Text.Json.Serialization;

namespace CodeSprint.Common.Dtos;

public class JwtDto
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }
    
    [JsonPropertyName("expiresIn")]
    public required int ExpiresIn { get; set; }
}