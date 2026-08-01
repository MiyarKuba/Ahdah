namespace Ahdah.Application.CompanyMembers.Models;

public sealed record CompanyMemberSummary(
    Guid MemberId,
    string FullName,
    string Role,
    string Status,
    string IdentityVerificationStatus,
    DateTimeOffset CreatedAtUtc);

public sealed record CompanyMemberDetails(
    Guid MemberId,
    string FullName,
    string Role,
    string Status,
    string IdentityVerificationStatus,
    string PhoneNumber,
    string? Email,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
