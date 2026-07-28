using System.Text;

namespace Ahdah.Api.Authentication;

public sealed record JwtOptions(
    string Issuer,
    string Audience,
    int AccessTokenMinutes,
    string SigningKey)
{
    public static JwtOptions FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection("Authentication:Jwt");
        var issuer = section["Issuer"];
        var audience = section["Audience"];
        var signingKey = section["SigningKey"];

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("Authentication:Jwt:Issuer is not configured.");
        }

        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("Authentication:Jwt:Audience is not configured.");
        }

        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("Authentication:Jwt:SigningKey is not configured.");
        }

        if (Encoding.UTF8.GetByteCount(signingKey) < 32)
        {
            throw new InvalidOperationException(
                "Authentication:Jwt:SigningKey must contain at least 32 bytes of UTF-8 key material.");
        }

        if (!int.TryParse(section["AccessTokenMinutes"], out var accessTokenMinutes)
            || accessTokenMinutes is < 1 or > 60)
        {
            throw new InvalidOperationException(
                "Authentication:Jwt:AccessTokenMinutes must be between 1 and 60.");
        }

        return new JwtOptions(issuer, audience, accessTokenMinutes, signingKey);
    }
}
