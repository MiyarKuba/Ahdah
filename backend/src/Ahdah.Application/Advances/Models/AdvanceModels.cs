using Ahdah.Application.Access.Models;

namespace Ahdah.Application.Advances.Models;

public sealed record AdvanceUserSummary(Guid UserId, string FullName, string Role);

public sealed record AdvanceSummary(
    Guid AdvanceId,
    string AdvanceNumber,
    decimal AdvanceAmount,
    decimal AvailableAmount,
    decimal ReservedAmount,
    string CurrencyCode,
    DateOnly IssueDate,
    DateOnly? SettlementDueDate,
    string Purpose,
    string? Notes,
    string Status,
    AdvanceUserSummary Recipient,
    int VersionNumber,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ConfirmedAtUtc);

public sealed record AdvanceFundingSummary(
    string SourceType,
    decimal AllocatedAmount,
    string FundingStatus,
    IReadOnlyList<string> PaymentMethods,
    DateTimeOffset AllocatedAtUtc);

public sealed record AdvanceBalanceSummary(
    Guid AdvanceId,
    string AdvanceNumber,
    AdvanceUserSummary Holder,
    decimal TotalReceivedAmount,
    decimal TotalRestoredAmount,
    decimal TotalExpensedAmount,
    decimal TotalTransferredOutAmount,
    decimal TotalReturnedAmount,
    decimal AvailableAmount,
    decimal ReservedAmount,
    string CurrencyCode,
    string Status,
    int VersionNumber,
    DateTimeOffset UpdatedAtUtc);

public sealed record AdvanceDetails(
    AdvanceSummary Advance,
    IReadOnlyList<AdvanceFundingSummary> Fundings,
    IReadOnlyList<AdvanceBalanceSummary> Balances,
    string? ClosureStatus);

public sealed record AdvanceMovementSummary(
    Guid MovementId,
    string OperationType,
    decimal Amount,
    AdvanceUserSummary Actor,
    AdvanceUserSummary? Sender,
    AdvanceUserSummary? Recipient,
    string Status,
    DateTimeOffset OccurredAtUtc);

public sealed record AdvanceTransferDetails(
    Guid TransferId,
    string TransferNumber,
    string TransferType,
    Guid AdvanceId,
    decimal Amount,
    string CurrencyCode,
    AdvanceUserSummary Sender,
    AdvanceUserSummary Recipient,
    string TransferMethod,
    string Status,
    int VersionNumber,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ConfirmedAtUtc,
    DateTimeOffset? RejectedAtUtc);

public sealed record AvailableFundingSourceSummary(
    Guid FundingSourceId,
    string SourceType,
    DateOnly SourceDate,
    string CurrencyCode,
    decimal AvailableAmount,
    string Status,
    IReadOnlyList<string> PaymentMethods);

public sealed record AdvanceBalancePage(PagedResult<AdvanceBalanceSummary> Page, decimal TotalAvailableAmount);
