using Ahdah.Application.Access;
using Ahdah.Application.Identity;
using Ahdah.Application.Suppliers.Contracts;

namespace Ahdah.Application.Suppliers;

public static class SupplierConstants
{
    public const string CashOnlyTransactionMode = "CashOnly";
    public const string CreditOnlyTransactionMode = "CreditOnly";
    public const string CashAndCreditTransactionMode = "CashAndCredit";

    public const string PendingVerificationStatus = "PendingVerification";
    public const string VerifiedStatus = "Verified";
    public const string RejectedStatus = "Rejected";

    public const string DraftStatus = "Draft";
    public const string PendingApprovalStatus = "PendingApproval";
    public const string ApprovedStatus = "Approved";
    public const string ConfirmedStatus = "Confirmed";
    public const string CancelledStatus = "Cancelled";
    public const string ReversedStatus = "Reversed";

    public const string OpenDebtStatus = "Open";
    public const string PartiallySettledDebtStatus = "PartiallySettled";
    public const string SettledDebtStatus = "Settled";

    public const string SupplierCreditPaymentMode = "SupplierCredit";
    public const string AvailableFundingStatus = "Available";
    public const string PartiallyUsedFundingStatus = "PartiallyUsed";
    public const string FullyUsedFundingStatus = "FullyUsed";
    public const string ManagerContributionSourceType = "ManagerContribution";
    public const string CompanyCashboxSourceType = "CompanyCashbox";

    public const string DebtCreatedLedgerType = "DebtCreated";
    public const string PaymentAppliedLedgerType = "PaymentApplied";
    public const string CreditNoteAppliedLedgerType = "CreditNoteApplied";
    public const string AmountReservedLedgerType = "AmountReserved";
    public const string SupplierPaymentLedgerType = "SupplierPayment";

    public const int MaximumSearchLength = 50;
    public const int MinimumIdempotencyKeyLength = 16;
    public const int MaximumIdempotencyKeyLength = 200;
    public const decimal MaximumMoneyAmount = 9999999999999999.99m;

    public static readonly IReadOnlySet<string> SupplierTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "GeneralSupplier", "MaterialsSupplier", "EquipmentSupplier", "EquipmentRental",
        "FuelSupplier", "Subcontractor", "TransportProvider", "MaintenanceProvider",
        "ServiceProvider", "Other"
    };

    public static readonly IReadOnlySet<string> TransactionModes = new HashSet<string>(StringComparer.Ordinal)
    {
        CashOnlyTransactionMode, CreditOnlyTransactionMode, CashAndCreditTransactionMode
    };

    public static readonly IReadOnlySet<string> PaymentMethods = new HashSet<string>(StringComparer.Ordinal)
    {
        "Cash", "BankTransfer", "Cheque", "Card", "MobileWallet", "Other"
    };

    public static readonly IReadOnlySet<string> AccountTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "BankAccount", "MobileWallet", "CashCollection", "Other"
    };

    public static readonly IReadOnlySet<string> DebtStatuses = new HashSet<string>(StringComparer.Ordinal)
    {
        OpenDebtStatus, PartiallySettledDebtStatus, SettledDebtStatus, CancelledStatus, ReversedStatus
    };

    public static readonly IReadOnlySet<string> PaymentStatuses = new HashSet<string>(StringComparer.Ordinal)
    {
        DraftStatus, PendingApprovalStatus, ConfirmedStatus, RejectedStatus, CancelledStatus, ReversedStatus
    };

    public static readonly IReadOnlySet<string> RefundStatuses = new HashSet<string>(StringComparer.Ordinal)
    {
        PendingVerificationStatus, ConfirmedStatus, CancelledStatus, ReversedStatus
    };

    public static readonly IReadOnlySet<string> CreditNoteStatuses = new HashSet<string>(StringComparer.Ordinal)
    {
        DraftStatus, PendingApprovalStatus, ApprovedStatus, RejectedStatus, CancelledStatus, ReversedStatus
    };

    public static readonly IReadOnlySet<string> CreditNoteReasonTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "ReturnedGoods", "DamagedGoods", "PricingCorrection", "Overbilling",
        "AdditionalDiscount", "ServiceCompensation", "Other"
    };

    public static readonly IReadOnlySet<string> UnitCodes = new HashSet<string>(StringComparer.Ordinal)
    {
        "Piece", "Package", "Box", "Bag", "Kilogram", "Ton", "Meter", "SquareMeter",
        "CubicMeter", "Liter", "Hour", "Day", "Trip", "Service", "LumpSum", "Other"
    };
}

public sealed record SupplierRoleCapabilities(
    bool CanViewCompanyFinancials,
    bool CanViewAssignedProjectSuppliers,
    bool CanManageSuppliers,
    bool CanManagePaymentAccounts,
    bool CanCreateInvoices,
    bool CanRecordPayments,
    bool CanManageCredits)
{
    public bool CanViewSuppliers => CanViewCompanyFinancials || CanViewAssignedProjectSuppliers;

    public static SupplierRoleCapabilities For(string? role) => role switch
    {
        IdentityConstants.ManagerRole => new(true, false, true, true, true, true, true),
        AccessConstants.DeputyRole => new(true, false, false, false, true, false, false),
        AccessConstants.AccountantRole => new(true, false, false, true, true, true, true),
        AccessConstants.SupervisorRole => new(false, true, false, false, false, false, false),
        _ => new(false, false, false, false, false, false, false)
    };
}

public static class SupplierRules
{
    public const bool SupportsAdvanceBalanceFunding = false;
    public const bool SupportsSupplierRefundCreation = false;
    public const bool SupportsHardDelete = false;
    public const bool SupportsFinalSettlement = false;

    public static bool IsValidMoney(decimal amount) =>
        amount > 0m
        && amount <= SupplierConstants.MaximumMoneyAmount
        && decimal.Truncate(amount * 100m) == amount * 100m;

    public static bool IsValidQuantity(decimal quantity) =>
        quantity > 0m
        && quantity <= 999999999999999.999m
        && decimal.Truncate(quantity * 1000m) == quantity * 1000m;

    public static bool IsValidNonnegativeMoney(decimal amount) =>
        amount >= 0m
        && amount <= SupplierConstants.MaximumMoneyAmount
        && decimal.Truncate(amount * 100m) == amount * 100m;

    public static bool IsValidIdempotencyKey(string? key) =>
        key is not null
        && key.Length is >= SupplierConstants.MinimumIdempotencyKeyLength
            and <= SupplierConstants.MaximumIdempotencyKeyLength
        && key.Trim() == key
        && !key.Any(char.IsControl);

    public static bool PaymentAllocationsMatch(CreateSupplierPaymentRequest request) =>
        UniquePositiveTotal(request.DebtAllocations, value => value.SupplierDebtId, value => value.Amount,
            request.PaymentAmount)
        && UniquePositiveTotal(request.FundingAllocations, value => value.FundingSourceId, value => value.Amount,
            request.PaymentAmount);

    public static bool CreditAllocationsAreValid(IReadOnlyList<SupplierCreditAllocationRequest> allocations) =>
        allocations.Count is >= 1 and <= 100
        && allocations.All(value => value.SupplierDebtId != Guid.Empty && IsValidMoney(value.Amount))
        && allocations.Select(value => value.SupplierDebtId).Distinct().Count() == allocations.Count;

    public static bool InvoiceItemsMatch(IReadOnlyList<SupplierInvoiceItemRequest> items, decimal amount)
    {
        if (items.Count == 0)
        {
            return true;
        }

        if (items.Count > 100
            || items.Any(item => !IsValidQuantity(item.Quantity)
                || !IsValidMoney(item.UnitPrice)
                || !IsValidNonnegativeMoney(item.DiscountAmount)
                || !IsValidNonnegativeMoney(item.TaxAmount)
                || !SupplierConstants.UnitCodes.Contains(item.UnitCode)
                || item.UnitCode == "Other" && string.IsNullOrWhiteSpace(item.CustomUnitName)
                || item.UnitCode != "Other" && item.CustomUnitName is not null
                || item.DiscountAmount > decimal.Round(item.Quantity * item.UnitPrice, 2, MidpointRounding.ToEven)
                || decimal.Round(item.Quantity * item.UnitPrice, 2, MidpointRounding.ToEven)
                    - item.DiscountAmount + item.TaxAmount <= 0m))
        {
            return false;
        }

        try
        {
            return items.Sum(item =>
            {
                var subtotal = decimal.Round(item.Quantity * item.UnitPrice, 2, MidpointRounding.ToEven);
                return subtotal - item.DiscountAmount + item.TaxAmount;
            }) == amount;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    private static bool UniquePositiveTotal<T>(
        IReadOnlyList<T> values,
        Func<T, Guid> id,
        Func<T, decimal> amount,
        decimal expected)
    {
        if (values.Count is < 1 or > 100
            || values.Any(value => id(value) == Guid.Empty || !IsValidMoney(amount(value)))
            || values.Select(id).Distinct().Count() != values.Count)
        {
            return false;
        }

        try
        {
            return values.Aggregate(0m, (total, value) => checked(total + amount(value))) == expected;
        }
        catch (OverflowException)
        {
            return false;
        }
    }
}
