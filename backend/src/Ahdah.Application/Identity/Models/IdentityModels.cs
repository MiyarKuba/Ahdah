namespace Ahdah.Application.Identity.Models;

public sealed record CompanySummary(
    Guid CompanyId,
    string CompanyName,
    string CompanyCode,
    string Status);

public sealed record UserSummary(
    Guid UserId,
    string FullName,
    string Role,
    string Status);

public sealed record AuthenticationResult(
    string AccessToken,
    string TokenType,
    DateTimeOffset AccessTokenExpiresAtUtc,
    UserSummary User,
    CompanySummary Company,
    string Role);

public sealed record RegisterCompanyResult(
    CompanySummary Company,
    UserSummary Manager,
    AuthenticationResult Authentication);

public sealed record CurrentUserResult(
    UserSummary User,
    CompanySummary Company,
    string Role);

public enum IdentityResultStatus
{
    Success,
    InvalidCredentials,
    Unauthorized,
    Conflict
}

public sealed record IdentityResult<T>(IdentityResultStatus Status, T? Value = default)
{
    public static IdentityResult<T> Success(T value) => new(IdentityResultStatus.Success, value);

    public static IdentityResult<T> Failure(IdentityResultStatus status) => new(status);
}
