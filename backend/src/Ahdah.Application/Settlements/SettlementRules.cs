using Ahdah.Application.Projects;

namespace Ahdah.Application.Settlements;

/// <summary>Read-side findings are not authority to execute a financial transition.</summary>
public static class SettlementRules
{
    public static ProjectSettlementSummary Summarize(
        Guid projectId, string status, int version, DateTime evaluatedAt, bool restricted,
        IReadOnlyList<SettlementCategorySummary> categories,
        IReadOnlyList<SettlementEvaluationGap> gaps)
    {
        var count = categories.Sum(category => category.BlockerCount);
        var settlementImpediments = new List<string>();
        var closureImpediments = new List<string>();
        if (count > 0)
        {
            settlementImpediments.Add("KnownFinancialBlockers");
            closureImpediments.Add("KnownFinancialBlockers");
        }

        if (status == ProjectConstants.CancelledStatus)
        {
            settlementImpediments.Add("ProjectCancelled");
            closureImpediments.Add("ProjectCancelled");
        }
        else if (status == ProjectConstants.FinanciallyClosedStatus)
        {
            settlementImpediments.Add("ProjectAlreadyFinanciallyClosed");
            closureImpediments.Add("ProjectAlreadyFinanciallyClosed");
        }
        else if (status is ProjectConstants.ActiveStatus or ProjectConstants.PausedStatus)
        {
            closureImpediments.Add("ProjectNotCompleted");
        }
        else if (!ProjectLifecycleRules.IsKnownStatus(status))
        {
            settlementImpediments.Add("UnknownProjectStatus");
            closureImpediments.Add("UnknownProjectStatus");
        }

        // No project-level settlement/closure transition policy exists in this schema.
        // Even zero known blockers cannot confer financial authority.
        bool? canSettle = settlementImpediments.Count > 0 ? false : null;
        bool? canClose = closureImpediments.Count > 0 ? false : null;
        return new(projectId, status, version, evaluatedAt,
            restricted ? "AssignedProjectLimited" : "CompanyFinancial",
            count > 0, canSettle, canClose,
            canSettle == false ? "Blocked" : "Indeterminate",
            canClose == false ? "Blocked" : "Indeterminate",
            count, categories, gaps, settlementImpediments, closureImpediments);
    }

    public static SettlementCategorySummary Category(
        string name, string amountMeaning, IEnumerable<ProjectSettlementBlocker> records,
        string evaluationStatus = "Evaluated")
    {
        var blockers = records.DistinctBy(row => (row.RecordType, row.RecordId, row.Code))
            .OrderBy(row => row.RecordType, StringComparer.Ordinal).ThenBy(row => row.RecordId)
            .ThenBy(row => row.Code, StringComparer.Ordinal).ToArray();
        var totals = blockers.Where(row => row.CurrencyCode is not null && row.Amount is not null)
            .GroupBy(row => row.CurrencyCode!, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => new SettlementCurrencyTotal(group.Key, group.Sum(row => row.Amount!.Value)))
            .ToArray();
        return new(name, evaluationStatus, amountMeaning, blockers.Length, totals, blockers);
    }
}
