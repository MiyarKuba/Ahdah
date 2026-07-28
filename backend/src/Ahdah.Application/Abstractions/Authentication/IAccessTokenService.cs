namespace Ahdah.Application.Abstractions.Authentication;

public interface IAccessTokenService
{
    AccessTokenResult CreateToken(AccessTokenSubject subject);
}

public sealed record AccessTokenSubject(
    Guid UserId,
    Guid CompanyId,
    string Role,
    string DisplayName);

public sealed record AccessTokenResult(
    string Token,
    DateTimeOffset ExpiresAtUtc);
