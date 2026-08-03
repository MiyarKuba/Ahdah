using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access;
using Ahdah.Application.Advances;
using Ahdah.Application.Advances.Contracts;
using Ahdah.Application.Identity;

namespace Ahdah.UnitTests;

public sealed class AdvanceRulesTests
{
    [Theory]
    [InlineData("Manager", true, true, true, true, true)]
    [InlineData("Deputy", true, false, true, true, true)]
    [InlineData("Accountant", true, false, false, false, true)]
    [InlineData("Supervisor", false, false, false, true, false)]
    [InlineData("Worker", false, false, false, true, false)]
    [InlineData("FutureRole", false, false, false, false, false)]
    public void Role_capability_matrix_is_conservative(
        string role,
        bool viewAll,
        bool create,
        bool distribute,
        bool returnFunds,
        bool viewOtherBalances)
    {
        var capabilities = AdvanceRoleCapabilities.For(role);

        Assert.Equal(viewAll, capabilities.CanViewAllCompanyAdvances);
        Assert.Equal(create, capabilities.CanCreateTopLevelAdvance);
        Assert.Equal(distribute, capabilities.CanDistributeHeldBalance);
        Assert.Equal(returnFunds, capabilities.CanReturnHeldBalance);
        Assert.Equal(viewOtherBalances, capabilities.CanViewOtherUserBalances);
        Assert.Equal(role == IdentityConstants.ManagerRole, capabilities.CanViewAvailableFundingSources);
        Assert.Equal(role is not (AccessConstants.AccountantRole or "FutureRole"),
            capabilities.CanConfirmReceipt);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1.001")]
    [InlineData("10000000000000000")]
    public void Money_validation_rejects_non_positive_excess_scale_and_precision(string value)
    {
        Assert.False(AdvanceRules.IsValidMoney(decimal.Parse(value)));
    }

    [Theory]
    [InlineData("0.01")]
    [InlineData("9999999999999999.99")]
    public void Money_validation_accepts_numeric_18_2_values(string value)
    {
        Assert.True(AdvanceRules.IsValidMoney(decimal.Parse(value)));
    }

    [Fact]
    public void Top_level_creation_requires_unique_allocations_that_equal_amount()
    {
        var sourceId = Guid.NewGuid();
        var request = ValidCreate() with
        {
            Amount = 100m,
            Fundings =
            [
                new AdvanceFundingAllocationRequest { FundingSourceId = sourceId, Amount = 50m },
                new AdvanceFundingAllocationRequest { FundingSourceId = sourceId, Amount = 40m }
            ]
        };

        Assert.Contains(Validate(request), result =>
            result.MemberNames.Contains(nameof(request.Fundings)));
    }

    [Theory]
    [InlineData("Deputy", "Active", true)]
    [InlineData("Supervisor", "Active", false)]
    [InlineData("Deputy", "Suspended", false)]
    public void Top_level_recipient_must_be_an_active_deputy(
        string role,
        string status,
        bool expected)
    {
        Assert.Equal(expected, AdvanceRules.IsEligibleTopLevelRecipient(role, status));
    }

    [Theory]
    [InlineData("Manager", "Deputy", "Active", true)]
    [InlineData("Manager", "Worker", "Active", false)]
    [InlineData("Deputy", "Supervisor", "Active", true)]
    [InlineData("Deputy", "Worker", "Active", true)]
    [InlineData("Deputy", "Worker", "Suspended", false)]
    [InlineData("Supervisor", "Worker", "Active", false)]
    [InlineData("Accountant", "Worker", "Active", false)]
    public void Distribution_recipient_follows_approved_role_chain(
        string senderRole,
        string recipientRole,
        string recipientStatus,
        bool expected)
    {
        Assert.Equal(expected, AdvanceRules.IsEligibleDistributionRecipient(
            senderRole,
            recipientRole,
            recipientStatus));
    }

    [Fact]
    public void Self_distribution_and_ambiguous_return_destinations_are_rejected()
    {
        var userId = Guid.NewGuid();

        Assert.False(AdvanceRules.IsDifferentUser(userId, userId));
        Assert.True(AdvanceRules.IsDifferentUser(userId, Guid.NewGuid()));
        Assert.True(AdvanceRules.HasUnambiguousReturnDestination([userId, userId]));
        Assert.False(AdvanceRules.HasUnambiguousReturnDestination([userId, Guid.NewGuid()]));
        Assert.False(AdvanceRules.HasUnambiguousReturnDestination([]));
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("PendingConfirmation")]
    [InlineData("Open")]
    [InlineData("InSettlement")]
    [InlineData("ReadyToClose")]
    [InlineData("Closed")]
    [InlineData("Cancelled")]
    [InlineData("Reversed")]
    public void Exact_advance_statuses_are_known(string status)
    {
        Assert.True(AdvanceRules.IsKnownAdvanceStatus(status));
    }

    [Fact]
    public void Unknown_statuses_fail_and_settlement_writes_remain_deferred()
    {
        Assert.False(AdvanceRules.IsKnownAdvanceStatus("Approved"));
        Assert.False(AdvanceRules.SupportsSettlementOrClosureWrites);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("short", false)]
    [InlineData(" 1234567890123456", false)]
    [InlineData("1234567890123456", true)]
    public void Financial_idempotency_keys_are_required_and_bounded(string? key, bool expected)
    {
        Assert.Equal(expected, AdvanceRules.IsValidIdempotencyKey(key));
    }

    [Theory]
    [InlineData("BankTransfer", null, "REF", null)]
    [InlineData("Card", null, null, null)]
    [InlineData("Other", null, null, null)]
    [InlineData("Wire", null, null, null)]
    public void Transfer_method_validation_rejects_missing_or_unknown_details(
        string method,
        string? bank,
        string? reference,
        string? description)
    {
        Assert.False(AdvanceRules.IsTransferMethodDetailValid(
            method,
            bank,
            reference,
            description));
    }

    private static CreateAdvanceRequest ValidCreate()
    {
        var sourceId = Guid.NewGuid();
        return new CreateAdvanceRequest
        {
            RecipientUserId = Guid.NewGuid(),
            Amount = 100m,
            IssueDate = new DateOnly(2026, 8, 3),
            TransferDate = new DateOnly(2026, 8, 3),
            TransferMethod = "Cash",
            Purpose = "Project cash custody",
            Fundings =
            [
                new AdvanceFundingAllocationRequest
                {
                    FundingSourceId = sourceId,
                    Amount = 100m
                }
            ]
        };
    }

    private static List<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), results, true);
        return results;
    }
}
