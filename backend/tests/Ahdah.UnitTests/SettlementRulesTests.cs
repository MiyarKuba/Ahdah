using Ahdah.Application.Expenses;
using Ahdah.Application.Settlements;

namespace Ahdah.UnitTests;

public sealed class SettlementRulesTests
{
    [Fact]
    public void Repeated_same_reason_is_counted_once_while_distinct_reasons_are_preserved()
    {
        var blocker = new ProjectSettlementBlocker("Expenses", "ExpensePendingReview", "Expense",
            Guid.NewGuid(), "PendingReview", "LYD", 0.10m, null);
        var category = SettlementRules.Category("Expenses", "PendingExpenseAmount", [blocker, blocker]);
        Assert.Equal(1, category.BlockerCount);
        Assert.Equal(0.10m, Assert.Single(category.Totals).Amount);
    }

    [Theory]
    [InlineData("Always", null, "0.01", true)]
    [InlineData("Threshold", "10.50", "10.50", true)]
    [InlineData("Threshold", "10.50", "10.49", false)]
    [InlineData("Threshold", null, "10.50", false)]
    [InlineData("Never", null, "10.50", false)]
    public void Shared_document_rule_preserves_existing_approval_behavior(string mode, string? threshold, string amount, bool expected)
    {
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        Assert.Equal(expected, ExpenseDocumentRules.IsRequired(mode,
            threshold is null ? null : decimal.Parse(threshold, culture), decimal.Parse(amount, culture)));
    }
}
