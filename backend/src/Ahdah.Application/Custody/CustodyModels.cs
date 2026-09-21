namespace Ahdah.Application.Custody;

public sealed record CustodyPositionDetails(
    Guid PositionId,
    Guid BalanceId,
    Guid? ProjectId,
    string? ProjectName,
    decimal AvailableAmount,
    decimal ReservedAmount,
    int Version);

public sealed record BalanceCustodyDetails(
    Guid BalanceId,
    Guid AdvanceId,
    string CurrencyCode,
    decimal BalanceAvailableAmount,
    decimal BalanceReservedAmount,
    IReadOnlyList<CustodyPositionDetails> Positions,
    bool IsReconciled);

public sealed record ProjectCustodyDetails(
    Guid ProjectId,
    string ProjectName,
    string FinancialState,
    long FinancialRevision,
    string AttributionStatus,
    IReadOnlyList<CustodyPositionDetails> Positions);

public sealed record CreateDesignationRequest(
    Guid BalanceId,
    Guid ProjectId,
    decimal Amount,
    string Reason);

public sealed record ApproveDesignationRequest(
    Guid BalanceId,
    Guid ProjectId,
    decimal Amount,
    string Reason,
    int ExpectedOperationVersion);

public sealed record CustodyOperationDetails(
    Guid OperationId,
    string OperationType,
    string Status,
    Guid RequestedByUserId,
    DateTimeOffset RequestedAtUtc,
    Guid? ApprovedByUserId,
    DateTimeOffset? ApprovedAtUtc,
    int Version,
    Guid? ProjectId,
    Guid? BalanceId,
    decimal? Amount,
    string? CurrencyCode,
    long? FinancialRevision);
