using Ahdah.Application.Suppliers;
using Ahdah.Application.Suppliers.Contracts;

namespace Ahdah.UnitTests;

public sealed class SupplierRulesTests
{
    [Theory]
    [InlineData("0", false)]
    [InlineData("-1", false)]
    [InlineData("1.001", false)]
    [InlineData("0.01", true)]
    [InlineData("9999999999999999.99", true)]
    public void Money_uses_numeric_18_2(string value, bool expected) =>
        Assert.Equal(expected, SupplierRules.IsValidMoney(decimal.Parse(value)));

    [Fact]
    public void Payment_requires_exact_unique_debt_and_funding_totals()
    {
        var debt = Guid.NewGuid();
        var source = Guid.NewGuid();
        var valid = Payment(100m,
            [new() { SupplierDebtId = debt, Amount = 100m }],
            [new() { FundingSourceId = source, Amount = 100m }]);
        var duplicate = Payment(100m,
            [new() { SupplierDebtId = debt, Amount = 50m }, new() { SupplierDebtId = debt, Amount = 50m }],
            [new() { FundingSourceId = source, Amount = 100m }]);

        Assert.True(SupplierRules.PaymentAllocationsMatch(valid));
        Assert.False(SupplierRules.PaymentAllocationsMatch(duplicate));
        Assert.False(SupplierRules.PaymentAllocationsMatch(valid with { PaymentAmount = 99.99m }));
    }

    [Fact]
    public void Invoice_items_use_quantity_18_3_and_exact_money_total()
    {
        var items = new[]
        {
            new SupplierInvoiceItemRequest
            {
                ItemName = "Cement", Quantity = 2.500m, UnitCode = "Bag",
                UnitPrice = 10m, DiscountAmount = 1m, TaxAmount = 1m
            }
        };
        Assert.True(SupplierRules.InvoiceItemsMatch(items, 25m));
        Assert.False(SupplierRules.InvoiceItemsMatch(items, 24.99m));
        Assert.False(SupplierRules.IsValidQuantity(1.0001m));
    }

    [Theory]
    [InlineData("Manager", true, true, true, true)]
    [InlineData("Deputy", true, false, true, false)]
    [InlineData("Accountant", true, false, true, true)]
    [InlineData("Supervisor", false, false, false, false)]
    [InlineData("Worker", false, false, false, false)]
    [InlineData("FutureRole", false, false, false, false)]
    public void Role_matrix_is_conservative(
        string role, bool viewCompany, bool manageSupplier, bool createInvoice, bool pay)
    {
        var capabilities = SupplierRoleCapabilities.For(role);
        Assert.Equal(viewCompany, capabilities.CanViewCompanyFinancials);
        Assert.Equal(manageSupplier, capabilities.CanManageSuppliers);
        Assert.Equal(createInvoice, capabilities.CanCreateInvoices);
        Assert.Equal(pay, capabilities.CanRecordPayments);
    }

    [Fact]
    public void Unsupported_financial_relationships_remain_explicitly_deferred()
    {
        Assert.False(SupplierRules.SupportsAdvanceBalanceFunding);
        Assert.False(SupplierRules.SupportsSupplierRefundCreation);
        Assert.False(SupplierRules.SupportsHardDelete);
        Assert.False(SupplierRules.SupportsFinalSettlement);
    }

    private static CreateSupplierPaymentRequest Payment(
        decimal amount,
        IReadOnlyList<SupplierPaymentDebtAllocationRequest> debts,
        IReadOnlyList<SupplierPaymentFundingAllocationRequest> funding) => new()
    {
        SupplierId = Guid.NewGuid(), PaymentDate = new DateOnly(2026, 8, 4),
        PaymentAmount = amount, CurrencyCode = "LYD", PaymentMethod = "Cash",
        DebtAllocations = debts, FundingAllocations = funding
    };
}
