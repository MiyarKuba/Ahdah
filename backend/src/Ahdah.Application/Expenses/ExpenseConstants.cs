using Ahdah.Application.Access;
using Ahdah.Application.Identity;

namespace Ahdah.Application.Expenses;

public static class ExpenseConstants
{
    public const string DraftStatus = "Draft";
    public const string PendingReviewStatus = "PendingReview";
    public const string CorrectionRequiredStatus = "CorrectionRequired";
    public const string ApprovedStatus = "Approved";
    public const string RejectedStatus = "Rejected";
    public const string CancelledStatus = "Cancelled";
    public const string ReversedStatus = "Reversed";

    public const string AdvanceBalancePaymentMode = "AdvanceBalance";
    public const string SupplierCreditPaymentMode = "SupplierCredit";
    public const string PersonalFundsPaymentMode = "PersonalFunds";

    public const string ProjectOnlyScope = "ProjectOnly";
    public const string CompanyOnlyScope = "CompanyOnly";
    public const string BothScope = "Both";

    public const string PendingVerificationStatus = "PendingVerification";
    public const string VerifiedDocumentStatus = "Verified";
    public const string RejectedDocumentStatus = "Rejected";

    public const string PersonalExpenseClaimSource = "PersonalExpense";
    public const string OpenClaimStatus = "Open";
    public const string PartiallySettledClaimStatus = "PartiallySettled";
    public const string SettledClaimStatus = "Settled";
    public const string CancelledClaimStatus = "Cancelled";
    public const string ReversedClaimStatus = "Reversed";

    public const string ExpenseReservedLedgerType = "ExpenseReserved";
    public const string ExpenseReservationReleasedLedgerType = "ExpenseReservationReleased";
    public const string ExpenseConfirmedLedgerType = "ExpenseConfirmed";

    public const int MaximumSearchLength = 50;
    public const int MinimumIdempotencyKeyLength = 16;
    public const int MaximumIdempotencyKeyLength = 200;
    public const decimal MaximumMoneyAmount = 9999999999999999.99m;

    public static readonly IReadOnlySet<string> CategoryGroups = new HashSet<string>(StringComparer.Ordinal)
    {
        "Materials", "Labor", "Subcontracting", "Transportation", "Equipment", "Fuel",
        "Services", "Administrative", "Utilities", "Permits", "Other"
    };

    public static readonly IReadOnlySet<string> CategoryScopes = new HashSet<string>(StringComparer.Ordinal)
    {
        ProjectOnlyScope, CompanyOnlyScope, BothScope
    };

    public static readonly IReadOnlySet<string> DocumentTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "Receipt", "Invoice", "Quotation", "DeliveryNote", "PaymentProof", "Contract",
        "PurchaseOrder", "Other"
    };

    public static readonly IReadOnlySet<string> CaptureSources = new HashSet<string>(StringComparer.Ordinal)
    {
        "Camera", "Gallery", "FileUpload", "Scanner", "Generated"
    };
}

public sealed record ExpenseRoleCapabilities(
    bool CanViewAllCompanyExpenses,
    bool CanCreateExpense,
    bool CanManageCategories,
    bool CanReviewExpense,
    bool CanAttachDocuments,
    bool CanViewAllReimbursements)
{
    public static ExpenseRoleCapabilities For(string? role) => role switch
    {
        IdentityConstants.ManagerRole => new(true, true, true, true, true, true),
        AccessConstants.DeputyRole => new(true, true, false, true, true, true),
        AccessConstants.AccountantRole => new(true, false, false, true, true, true),
        AccessConstants.SupervisorRole => new(false, true, false, false, true, false),
        AccessConstants.WorkerRole => new(false, true, false, false, true, false),
        _ => new(false, false, false, false, false, false)
    };
}

public static class ExpenseRules
{
    public const bool SupportsMultipleProjectAllocation = false;
    public const bool SupportsOriginalDocumentCustody = false;
    public const bool SupportsSupplierDebtWrites = false;
    public const bool SupportsFinalSettlement = false;

    public static bool IsKnownStatus(string? status) => status is
        ExpenseConstants.DraftStatus or ExpenseConstants.PendingReviewStatus or
        ExpenseConstants.CorrectionRequiredStatus or ExpenseConstants.ApprovedStatus or
        ExpenseConstants.RejectedStatus or ExpenseConstants.CancelledStatus or
        ExpenseConstants.ReversedStatus;

    public static bool IsKnownPaymentMode(string? paymentMode) => paymentMode is
        ExpenseConstants.AdvanceBalancePaymentMode or ExpenseConstants.SupplierCreditPaymentMode or
        ExpenseConstants.PersonalFundsPaymentMode;

    public static bool IsSupportedCreationPaymentMode(string? paymentMode) => paymentMode is
        ExpenseConstants.AdvanceBalancePaymentMode or ExpenseConstants.PersonalFundsPaymentMode;

    public static bool IsKnownClaimStatus(string? status) => status is
        ExpenseConstants.OpenClaimStatus or ExpenseConstants.PartiallySettledClaimStatus or
        ExpenseConstants.SettledClaimStatus or ExpenseConstants.CancelledClaimStatus or
        ExpenseConstants.ReversedClaimStatus;

    public static bool IsValidMoney(decimal amount) =>
        amount > 0m
        && amount <= ExpenseConstants.MaximumMoneyAmount
        && decimal.Truncate(amount * 100m) == amount * 100m;

    public static bool AllocationsMatch(
        IReadOnlyList<Contracts.ExpenseAdvanceAllocationRequest> allocations,
        decimal amount)
    {
        if (allocations.Count is < 1 or > 100
            || allocations.Any(allocation => allocation.UserAdvanceBalanceId == Guid.Empty
                || !IsValidMoney(allocation.Amount))
            || allocations.Select(allocation => allocation.UserAdvanceBalanceId).Distinct().Count()
                != allocations.Count)
        {
            return false;
        }

        try
        {
            return allocations.Aggregate(0m, (total, allocation) =>
                checked(total + allocation.Amount)) == amount;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    public static bool ProjectMatchesScope(string scope, Guid? projectId) => scope switch
    {
        ExpenseConstants.ProjectOnlyScope => projectId is not null && projectId != Guid.Empty,
        ExpenseConstants.CompanyOnlyScope => projectId is null,
        ExpenseConstants.BothScope => projectId is null || projectId != Guid.Empty,
        _ => false
    };

    public static bool IsValidIdempotencyKey(string? key) =>
        key is not null
        && key.Length is >= ExpenseConstants.MinimumIdempotencyKeyLength
            and <= ExpenseConstants.MaximumIdempotencyKeyLength
        && key.Trim() == key
        && !key.Any(char.IsControl);
}
