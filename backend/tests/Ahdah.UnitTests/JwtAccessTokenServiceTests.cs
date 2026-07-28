using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Ahdah.Api.Authentication;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Ahdah.UnitTests;

public sealed class JwtAccessTokenServiceTests
{
    private const string TestSigningKey = "unit-test-signing-key-material-at-least-32-bytes-long";
    private static readonly DateTimeOffset TestNow = new(2026, 7, 28, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Token_contains_only_required_identity_claims_and_expected_expiry()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var service = CreateService();

        var result = service.CreateToken(
            new AccessTokenSubject(userId, companyId, IdentityConstants.ManagerRole, "Test Manager"));
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        Assert.Equal(TestNow.AddMinutes(15), result.ExpiresAtUtc);
        Assert.Equal(userId.ToString(), token.Subject);
        Assert.Equal(companyId.ToString(), token.Claims.Single(c => c.Type == AhdahClaimTypes.CompanyId).Value);
        Assert.Equal(IdentityConstants.ManagerRole, token.Claims.Single(c => c.Type == AhdahClaimTypes.Role).Value);
        Assert.Equal("Test Manager", token.Claims.Single(c => c.Type == AhdahClaimTypes.Name).Value);
        Assert.Single(token.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        Assert.DoesNotContain(token.Claims, c => c.Type is "phone" or "email" or "password_hash");
    }

    [Fact]
    public void Token_signature_issuer_and_audience_validate()
    {
        var service = CreateService();
        var result = service.CreateToken(
            new AccessTokenSubject(Guid.NewGuid(), Guid.NewGuid(), IdentityConstants.ManagerRole, "Test Manager"));
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };

        var principal = handler.ValidateToken(
            result.Token,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "Ahdah.Api.Tests",
                ValidateAudience = true,
                ValidAudience = "Ahdah.Test.Client",
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey)),
                RequireExpirationTime = true,
                RequireSignedTokens = true
            },
            out var validatedToken);

        Assert.NotNull(principal.FindFirst(JwtRegisteredClaimNames.Sub));
        Assert.IsType<JwtSecurityToken>(validatedToken);
    }

    private static JwtAccessTokenService CreateService() => new(
        new JwtOptions("Ahdah.Api.Tests", "Ahdah.Test.Client", 15, TestSigningKey),
        new FixedTimeProvider(TestNow));

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
