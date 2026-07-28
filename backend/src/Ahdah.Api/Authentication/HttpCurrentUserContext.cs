using System.IdentityModel.Tokens.Jwt;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Identity;

namespace Ahdah.Api.Authentication;

public sealed class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserContext
{
    private System.Security.Claims.ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public Guid? UserId => ParseGuidClaim(JwtRegisteredClaimNames.Sub);

    public Guid? CompanyId => ParseGuidClaim(AhdahClaimTypes.CompanyId);

    public string? Role => IsAuthenticated
        ? User?.FindFirst(AhdahClaimTypes.Role)?.Value
        : null;

    private Guid? ParseGuidClaim(string claimType)
    {
        if (!IsAuthenticated)
        {
            return null;
        }

        return Guid.TryParse(User?.FindFirst(claimType)?.Value, out var value)
            ? value
            : null;
    }
}
