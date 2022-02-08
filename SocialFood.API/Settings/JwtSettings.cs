using System;
namespace SocialFood.API.Settings;

public class JwtSettings
{
    public string SecurityKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
}
