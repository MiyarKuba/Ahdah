using Ahdah.Application.Access.Models;

namespace Ahdah.Application.Settlements;

public interface IProjectSettlementService
{
    Task<AccessResult<ProjectSettlementSummary>> GetAsync(Guid projectId, CancellationToken cancellationToken);
}

public sealed record ProjectSettlementBlocker(
    string Category, string Code, string RecordType, Guid RecordId,
    string Status, string? CurrencyCode, decimal? Amount, string? ResourcePath);

public sealed record SettlementCurrencyTotal(string CurrencyCode, decimal Amount);

public sealed record SettlementCategorySummary(
    string Category, string EvaluationStatus, string AmountMeaning,
    int BlockerCount, IReadOnlyList<SettlementCurrencyTotal> Totals,
    IReadOnlyList<ProjectSettlementBlocker> Blockers);

public sealed record SettlementEvaluationGap(string Category, string Code);

public sealed record ProjectSettlementSummary(
    Guid ProjectId, string ProjectStatus, int ProjectVersion, DateTime EvaluatedAt,
    string VisibilityScope, bool HasKnownFinancialBlockers,
    bool? CanSettle, bool? CanClose, string SettlementReadiness, string ClosureReadiness,
    int TotalBlockerCount, IReadOnlyList<SettlementCategorySummary> Categories,
    IReadOnlyList<SettlementEvaluationGap> EvaluationGaps,
    IReadOnlyList<string> SettlementImpediments, IReadOnlyList<string> ClosureImpediments);
