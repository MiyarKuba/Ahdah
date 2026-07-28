namespace Ahdah.Application.Access;

public static class AccessConstants
{
    public const string DeputyRole = "Deputy";
    public const string AccountantRole = "Accountant";
    public const string SupervisorRole = "Supervisor";
    public const string WorkerRole = "Worker";

    public const string PendingInvitationStatus = "Pending";
    public const string AcceptedInvitationStatus = "Accepted";
    public const string ExpiredInvitationStatus = "Expired";
    public const string CancelledInvitationStatus = "Cancelled";

    public const string PendingJoinRequestStatus = "Pending";
    public const string ApprovedJoinRequestStatus = "Approved";
    public const string RejectedJoinRequestStatus = "Rejected";
    public const string CancelledJoinRequestStatus = "Cancelled";

    public const string PendingApprovalUserStatus = "PendingApproval";
    public const string ActiveUserStatus = "Active";
    public const string RejectedUserStatus = "Rejected";
    public const string PendingIdentityStatus = "Pending";
    public const string NotRequiredIdentityStatus = "NotRequired";
    public const string VerifiedIdentityStatus = "Verified";

    public const string AcceptedAndActivatedOutcome = "AcceptedAndActivated";
    public const string AcceptedPendingIdentityVerificationOutcome = "AcceptedPendingIdentityVerification";
    public const string ApprovedAndActivatedOutcome = "ApprovedAndActivated";
    public const string ApprovedPendingIdentityVerificationOutcome = "ApprovedPendingIdentityVerification";
    public const string RejectedOutcome = "Rejected";

    public const int DefaultPageSize = 20;
    public const int MaximumPageSize = 100;
}

public static class AccessRoleRules
{
    private static readonly HashSet<string> AssignableRoles = new(StringComparer.Ordinal)
    {
        AccessConstants.DeputyRole,
        AccessConstants.AccountantRole,
        AccessConstants.SupervisorRole,
        AccessConstants.WorkerRole
    };

    public static bool IsAssignable(string? role) =>
        role is not null && AssignableRoles.Contains(role.Trim());

    public static string Normalize(string role) => role.Trim();

    public static bool RequiresIdentityVerification(string role) =>
        role is AccessConstants.DeputyRole or AccessConstants.AccountantRole;

    public static string InitialIdentityStatus(string role) =>
        RequiresIdentityVerification(role)
            ? AccessConstants.PendingIdentityStatus
            : AccessConstants.NotRequiredIdentityStatus;

    public static string UserStatusForJoinRejection() => AccessConstants.RejectedUserStatus;

    public static UserActivationDecision ForInvitationAcceptance(string role) =>
        RequiresIdentityVerification(role)
            ? new UserActivationDecision(
                AccessConstants.PendingApprovalUserStatus,
                InitialIdentityStatus(role),
                AccessConstants.AcceptedPendingIdentityVerificationOutcome)
            : new UserActivationDecision(
                AccessConstants.ActiveUserStatus,
                InitialIdentityStatus(role),
                AccessConstants.AcceptedAndActivatedOutcome);

    public static UserActivationDecision ForJoinApproval(string role, string currentIdentityStatus) =>
        RequiresIdentityVerification(role)
            ? currentIdentityStatus == AccessConstants.VerifiedIdentityStatus
                ? new UserActivationDecision(
                    AccessConstants.ActiveUserStatus,
                    AccessConstants.VerifiedIdentityStatus,
                    AccessConstants.ApprovedAndActivatedOutcome)
                : new UserActivationDecision(
                    AccessConstants.PendingApprovalUserStatus,
                    AccessConstants.PendingIdentityStatus,
                    AccessConstants.ApprovedPendingIdentityVerificationOutcome)
            : new UserActivationDecision(
                AccessConstants.ActiveUserStatus,
                AccessConstants.NotRequiredIdentityStatus,
                AccessConstants.ApprovedAndActivatedOutcome);
}

public sealed record UserActivationDecision(string UserStatus, string IdentityStatus, string Outcome);
