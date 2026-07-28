using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Ahdah.Api.Authentication;

public sealed class JwtAccessTokenService(
    JwtOptions options,
    TimeProvider timeProvider) : IAccessTokenService
{
    public AccessTokenResult CreateToken(AccessTokenSubject subject)
    {
        var issuedAt = timeProvider.GetUtcNow();
        var expiresAt = issuedAt.AddMinutes(options.AccessTokenMinutes);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, subject.UserId.ToString()),
            new Claim(AhdahClaimTypes.CompanyId, subject.CompanyId.ToString()),
            new Claim(AhdahClaimTypes.Role, subject.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(AhdahClaimTypes.Name, subject.DisplayName)
        };

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}
