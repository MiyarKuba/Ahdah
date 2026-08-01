using System.Text.Json.Serialization;

namespace Ahdah.Application.Projects.Models;

public sealed record ProjectOwnerSummary(
    Guid ProjectOwnerId,
    string OwnerName,
    string PhoneNumber,
    string? Email,
    string? Address);

public sealed record ProjectSupervisorSummary(
    Guid UserId,
    string FullName,
    string Status,
    DateTimeOffset AssignedAtUtc);

public sealed record ProjectDetails(
    Guid ProjectId,
    string ProjectName,
    ProjectOwnerSummary Owner,
    string SiteAddress,
    decimal? Latitude,
    decimal? Longitude,
    string? ContactPhoneNumber,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] decimal? ContractValue,
    DateOnly ContractDate,
    DateOnly StartDate,
    DateOnly? ExpectedEndDate,
    DateOnly? ActualEndDate,
    string Status,
    string? Description,
    string? Notes,
    IReadOnlyList<ProjectSupervisorSummary> AssignedSupervisors,
    int VersionNumber,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record ProjectMemberSummary(
    Guid MemberId,
    string FullName,
    string Role,
    string Status,
    string IdentityVerificationStatus,
    string PhoneNumber,
    string? Email,
    DateTimeOffset AssignedAtUtc);
