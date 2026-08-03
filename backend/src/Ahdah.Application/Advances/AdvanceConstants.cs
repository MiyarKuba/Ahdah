using Ahdah.Application.Access;
using Ahdah.Application.Identity;

namespace Ahdah.Application.Advances;

public static class AdvanceConstants
{
    public const string DraftStatus = "Draft";
    public const string PendingConfirmationStatus = "PendingConfirmation";
    public const string OpenStatus = "Open";
    public const string InSettlementStatus = "InSettlement";
    public const string ReadyToCloseStatus = "ReadyToClose";
    public const string ClosedStatus = "Closed";
    public const string CancelledStatus = "Cancelled";
    public const string ReversedStatus = "Reversed";

    public const string AdvanceDeliveryTransferType = "AdvanceDelivery";
    public const string InternalTransferType = "InternalTransfer";
    public const string BalanceReturnTransferType = "BalanceReturn";

    public const string ConfirmedTransferStatus = "Confirmed";
    public const string RejectedTransferStatus = "Rejected";

    public const string ActiveBalanceStatus = "Active";
    public const string ActiveFundingStatus = "Available";
    public const string PartiallyUsedFundingStatus = "PartiallyUsed";
    public const string FullyUsedFundingStatus = "FullyUsed";

    public const string DefaultCurrencyCode = "LYD";
    public const int MaximumSearchLength = 50;
    public const int MinimumIdempotencyKeyLength = 16;
    public const int MaximumIdempotencyKeyLength = 200;
    public const decimal MaximumMoneyAmount = 9999999999999999.99m;
}

public sealed record AdvanceRoleCapabilities(
    bool CanViewAllCompanyAdvances,
    bool CanCreateTopLevelAdvance,
    bool CanDistributeHeldBalance,
    bool CanConfirmReceipt,
    bool CanReturnHeldBalance,
    bool CanViewOtherUserBalances,
    bool CanViewAvailableFundingSources)
{
    public static AdvanceRoleCapabilities For(string? role) => role switch
    {
        IdentityConstants.ManagerRole => new(true, true, true, true, true, true, true),
        AccessConstants.DeputyRole => new(true, false, true, true, true, true, false),
        AccessConstants.AccountantRole => new(true, false, false, false, false, true, false),
        AccessConstants.SupervisorRole => new(false, false, false, true, true, false, false),
        AccessConstants.WorkerRole => new(false, false, false, true, true, false, false),
        _ => new(false, false, false, false, false, false, false)
    };
}

public static class AdvanceRules
{
    public const bool SupportsSettlementOrClosureWrites = false;

    public static bool IsKnownAdvanceStatus(string? status) => status is
        AdvanceConstants.DraftStatus or
        AdvanceConstants.PendingConfirmationStatus or
        AdvanceConstants.OpenStatus or
        AdvanceConstants.InSettlementStatus or
        AdvanceConstants.ReadyToCloseStatus or
        AdvanceConstants.ClosedStatus or
        AdvanceConstants.CancelledStatus or
        AdvanceConstants.ReversedStatus;

    public static bool IsValidMoney(decimal amount) =>
        amount > 0m
        && amount <= AdvanceConstants.MaximumMoneyAmount
        && decimal.Truncate(amount * 100m) == amount * 100m;

    public static bool IsKnownTransferMethod(string? method) => method is
        "Cash" or "BankTransfer" or "Cheque" or "Card" or "MobileWallet" or
        "BalanceTransfer" or "Other";

    public static bool IsTransferMethodDetailValid(
        string? method,
        string? bankName,
        string? referenceNumber,
        string? description)
    {
        if (!IsKnownTransferMethod(method))
        {
            return false;
        }

        if (method is "BankTransfer" or "Cheque"
            && (string.IsNullOrWhiteSpace(bankName) || string.IsNullOrWhiteSpace(referenceNumber)))
        {
            return false;
        }

        if (method is "Card" or "MobileWallet" && string.IsNullOrWhiteSpace(referenceNumber))
        {
            return false;
        }

        return method != "Other" || !string.IsNullOrWhiteSpace(description);
    }

    public static bool IsEligibleTopLevelRecipient(string? role, string? status) =>
        role == AccessConstants.DeputyRole && status == AccessConstants.ActiveUserStatus;

    public static bool IsEligibleDistributionRecipient(
        string senderRole,
        string? recipientRole,
        string? recipientStatus) =>
        recipientStatus == AccessConstants.ActiveUserStatus
        && senderRole switch
        {
            IdentityConstants.ManagerRole => recipientRole == AccessConstants.DeputyRole,
            AccessConstants.DeputyRole => recipientRole is
                AccessConstants.SupervisorRole or AccessConstants.WorkerRole,
            _ => false
        };

    public static bool IsDifferentUser(Guid senderUserId, Guid recipientUserId) =>
        senderUserId != Guid.Empty
        && recipientUserId != Guid.Empty
        && senderUserId != recipientUserId;

    public static bool HasUnambiguousReturnDestination(IEnumerable<Guid> upstreamSenderIds) =>
        upstreamSenderIds.Distinct().Take(2).Count() == 1;

    public static bool FundingTotalMatches(
        IReadOnlyList<Contracts.AdvanceFundingAllocationRequest> fundings,
        decimal amount)
    {
        if (fundings.Count is < 1 or > 100)
        {
            return false;
        }

        try
        {
            return fundings.Aggregate(0m, (total, funding) => checked(total + funding.Amount)) == amount;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    public static bool IsValidIdempotencyKey(string? key) =>
        key is not null
        && key.Length is >= AdvanceConstants.MinimumIdempotencyKeyLength
            and <= AdvanceConstants.MaximumIdempotencyKeyLength
        && key.Trim() == key
        && !key.Any(char.IsControl);
}
