using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Authentication;

public readonly struct AuthToken()
{
    [JsonPropertyName("token")] public string Token { get; init; } = string.Empty;

    [JsonPropertyName("token_type")] public string TokenType { get; init; } = "Bearer";

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; init; } = 0;
}