using Ahdah.Application.Access;
using Ahdah.Application.Expenses;
using Ahdah.Application.Expenses.Contracts;
using Ahdah.Application.Identity;

namespace Ahdah.UnitTests;

public sealed class ExpenseRulesTests
{
    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1.001")]
    [InlineData("10000000000000000")]
    public void Money_rejects_non_positive_over_scale_or_over_precision(string text) =>
        Assert.False(ExpenseRules.IsValidMoney(decimal.Parse(text)));

    [Theory]
    [InlineData("0.01")]
    [InlineData("10.20")]
    [InlineData("9999999999999999.99")]
    public void Money_accepts_numeric_18_2_values(string text) =>
        Assert.True(ExpenseRules.IsValidMoney(decimal.Parse(text)));

    [Fact]
    public void Advance_allocations_must_be_unique_and_total_exactly()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var valid = new[]
        {
            new ExpenseAdvanceAllocationRequest { UserAdvanceBalanceId = first, Amount = 30.10m },
            new ExpenseAdvanceAllocationRequest { UserAdvanceBalanceId = second, Amount = 69.90m }
        };
        var duplicate = new[]
        {
            new ExpenseAdvanceAllocationRequest { UserAdvanceBalanceId = first, Amount = 50m },
            new ExpenseAdvanceAllocationRequest { UserAdvanceBalanceId = first, Amount = 50m }
        };

        Assert.True(ExpenseRules.AllocationsMatch(valid, 100m));
        Assert.False(ExpenseRules.AllocationsMatch(valid, 99.99m));
        Assert.False(ExpenseRules.AllocationsMatch(duplicate, 100m));
    }

    [Theory]
    [InlineData("ProjectOnly", true, true)]
    [InlineData("ProjectOnly", false, false)]
    [InlineData("CompanyOnly", true, false)]
    [InlineData("CompanyOnly", false, true)]
    [InlineData("Both", true, true)]
    [InlineData("Both", false, true)]
    public void Project_requirement_follows_exact_category_scope(
        string scope,
        bool hasProject,
        bool expected) => Assert.Equal(
            expected,
            ExpenseRules.ProjectMatchesScope(scope, hasProject ? Guid.NewGuid() : null));

    [Theory]
    [InlineData("Manager", true, true, true, true)]
    [InlineData("Deputy", true, true, false, true)]
    [InlineData("Accountant", true, false, false, true)]
    [InlineData("Supervisor", false, true, false, false)]
    [InlineData("Worker", false, true, false, false)]
    [InlineData("FutureRole", false, false, false, false)]
    public void Role_capabilities_are_conservative(
        string role,
        bool viewAll,
        bool create,
        bool categories,
        bool review)
    {
        var capabilities = ExpenseRoleCapabilities.For(role);

        Assert.Equal(viewAll, capabilities.CanViewAllCompanyExpenses);
        Assert.Equal(create, capabilities.CanCreateExpense);
        Assert.Equal(categories, capabilities.CanManageCategories);
        Assert.Equal(review, capabilities.CanReviewExpense);
    }

    [Fact]
    public void Exact_schema_values_and_deferred_workflows_are_explicit()
    {
        Assert.True(ExpenseRules.IsKnownStatus(ExpenseConstants.PendingReviewStatus));
        Assert.True(ExpenseRules.IsKnownPaymentMode(ExpenseConstants.SupplierCreditPaymentMode));
        Assert.False(ExpenseRules.IsSupportedCreationPaymentMode(
            ExpenseConstants.SupplierCreditPaymentMode));
        Assert.False(ExpenseRules.SupportsMultipleProjectAllocation);
        Assert.False(ExpenseRules.SupportsOriginalDocumentCustody);
        Assert.True(ExpenseRules.SupportsSupplierDebtWrites);
        Assert.False(ExpenseRules.SupportsFinalSettlement);
    }

    [Theory]
    [InlineData("short", false)]
    [InlineData("1234567890123456", true)]
    [InlineData(" 1234567890123456", false)]
    public void Idempotency_key_uses_existing_length_and_format_rule(string key, bool expected) =>
        Assert.Equal(expected, ExpenseRules.IsValidIdempotencyKey(key));
}
