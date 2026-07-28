namespace Ahdah.Application.Access.Models;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record InvitationSummary(
    Guid InvitationId,
    string InvitedPhoneNumber,
    string AssignedRole,
    string Status,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? AcceptedAtUtc,
    DateTimeOffset? CancelledAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record CreateInvitationResult(
    InvitationSummary Invitation,
    string Token);

public sealed record InvitationAcceptanceResult(
    string Outcome,
    string UserStatus,
    string AssignedRole,
    Ahdah.Application.Identity.Models.AuthenticationResult? Authentication);

public sealed record JoinRequestSubmissionResult(string Status);

public sealed record JoinRequestSummary(
    Guid JoinRequestId,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string? Email,
    string RequestedRole,
    string? AssignedRole,
    string Status,
    string? RequestMessage,
    string? ReviewNotes,
    string? RejectionReason,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? ReviewedAtUtc,
    DateTimeOffset? CancelledAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record JoinRequestDecisionResult(
    string Outcome,
    string UserStatus,
    JoinRequestSummary Request);

public enum AccessResultStatus
{
    Success,
    Invalid,
    NotFound,
    Conflict,
    Unauthorized
}

public sealed record AccessResult<T>(AccessResultStatus Status, T? Value = default)
{
    public static AccessResult<T> Success(T value) => new(AccessResultStatus.Success, value);

    public static AccessResult<T> Failure(AccessResultStatus status) => new(status);
}
