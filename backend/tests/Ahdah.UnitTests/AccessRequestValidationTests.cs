using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access;
using Ahdah.Application.Access.Contracts;

namespace Ahdah.UnitTests;

public sealed class AccessRequestValidationTests
{
    [Theory]
    [InlineData("Deputy")]
    [InlineData("Accountant")]
    [InlineData("Supervisor")]
    [InlineData("Worker")]
    [InlineData("  Worker  ")]
    public void Approval_accepts_supported_non_manager_roles(string role)
    {
        var request = new ApproveJoinRequestRequest(role, null);

        Assert.True(AccessRoleRules.IsAssignable(role));
        Assert.Empty(Validate(request));
    }

    [Fact]
    public void Approval_rejects_manager_role()
    {
        var request = new ApproveJoinRequestRequest("Manager", null);

        Assert.False(AccessRoleRules.IsAssignable(request.AssignedRole));
        Assert.Contains(Validate(request), result =>
            result.MemberNames.Contains(nameof(request.AssignedRole)));
    }

    [Fact]
    public void Invitation_creation_rejects_manager_role()
    {
        var request = new CreateInvitationRequest("+218912345678", "Manager");

        Assert.Contains(Validate(request), result =>
            result.MemberNames.Contains(nameof(request.AssignedRole)));
    }

    [Fact]
    public void Join_submission_rejects_manager_and_invalid_onboarding_fields()
    {
        var request = new SubmitJoinRequestRequest(
            "bad",
            "Applicant",
            "0912345678",
            null,
            "short",
            "Manager",
            null);

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.CompanyCode)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.PhoneNumber)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Password)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.RequestedRole)));
    }

    [Fact]
    public void Invitation_acceptance_rejects_short_token_and_password()
    {
        var request = new AcceptInvitationRequest("short", "Test User", "short", null);

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Token)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Password)));
    }

    [Fact]
    public void Pagination_rejects_zero_page_and_oversized_page_size()
    {
        var query = new InvitationQuery { Page = 0, PageSize = AccessConstants.MaximumPageSize + 1 };

        var results = Validate(query);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.Page)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(query.PageSize)));
    }

    [Fact]
    public void Join_request_rejection_requires_a_non_blank_reason()
    {
        var request = new RejectJoinRequestRequest("   ");

        Assert.Contains(Validate(request), result => result.MemberNames.Contains(nameof(request.Reason)));
    }

    private static List<ValidationResult> Validate(object value)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);
        return validationResults;
    }
}
