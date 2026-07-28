using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access;
using Ahdah.Infrastructure.Access.Invitations;

namespace Ahdah.UnitTests;

public sealed class AccessWorkflowRulesTests
{
    [Fact]
    public void Invitation_options_accept_168_hours()
    {
        var options = new InvitationOptions { LifetimeHours = 168 };

        Assert.Empty(Validate(options));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(721)]
    public void Invitation_options_reject_invalid_lifetimes(int lifetimeHours)
    {
        var options = new InvitationOptions { LifetimeHours = lifetimeHours };

        Assert.NotEmpty(Validate(options));
    }

    [Fact]
    public void Invitation_expiry_uses_time_provider_and_configured_lifetime()
    {
        var now = DateTimeOffset.Parse("2026-07-28T10:15:00Z");
        var timeProvider = new FixedTimeProvider(now);
        var options = new InvitationOptions { LifetimeHours = 168 };

        var expiresAt = InvitationExpiryCalculator.Calculate(timeProvider, options);

        Assert.Equal(now.AddHours(168).UtcDateTime, expiresAt);
    }

    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    public void Sensitive_requested_roles_start_with_pending_identity(string role)
    {
        Assert.Equal(AccessConstants.PendingIdentityStatus, AccessRoleRules.InitialIdentityStatus(role));
    }

    [Theory]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    public void Non_sensitive_requested_roles_start_with_not_required_identity(string role)
    {
        Assert.Equal(AccessConstants.NotRequiredIdentityStatus, AccessRoleRules.InitialIdentityStatus(role));
    }

    [Theory]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    public void Invitation_acceptance_activates_non_sensitive_roles(string role)
    {
        var decision = AccessRoleRules.ForInvitationAcceptance(role);

        Assert.Equal(AccessConstants.ActiveUserStatus, decision.UserStatus);
        Assert.Equal(AccessConstants.NotRequiredIdentityStatus, decision.IdentityStatus);
    }

    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    public void Invitation_acceptance_keeps_sensitive_roles_pending(string role)
    {
        var decision = AccessRoleRules.ForInvitationAcceptance(role);

        Assert.Equal(AccessConstants.PendingApprovalUserStatus, decision.UserStatus);
        Assert.Equal(AccessConstants.PendingIdentityStatus, decision.IdentityStatus);
    }

    [Theory]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    public void Approval_activates_non_sensitive_roles(string role)
    {
        var decision = AccessRoleRules.ForJoinApproval(role, AccessConstants.NotRequiredIdentityStatus);

        Assert.Equal(AccessConstants.ActiveUserStatus, decision.UserStatus);
        Assert.Equal(AccessConstants.ApprovedAndActivatedOutcome, decision.Outcome);
    }

    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    public void Approval_keeps_unverified_sensitive_roles_pending(string role)
    {
        var decision = AccessRoleRules.ForJoinApproval(role, AccessConstants.PendingIdentityStatus);

        Assert.Equal(AccessConstants.PendingApprovalUserStatus, decision.UserStatus);
        Assert.Equal(AccessConstants.ApprovedPendingIdentityVerificationOutcome, decision.Outcome);
    }

    [Fact]
    public void Rejection_maps_linked_user_to_rejected()
    {
        Assert.Equal(AccessConstants.RejectedUserStatus, AccessRoleRules.UserStatusForJoinRejection());
    }

    private static List<ValidationResult> Validate(object value)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);
        return validationResults;
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
