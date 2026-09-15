using System.Data;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access.Models;
using Ahdah.Application.Expenses;
using Ahdah.Application.Identity;
using Ahdah.Application.Projects;
using Ahdah.Application.Settlements;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.Infrastructure.Settlements;

public sealed class ProjectSettlementService(
    AhdahDbContext db, ICurrentUserContext currentUser, TimeProvider timeProvider) : IProjectSettlementService
{
    public async Task<AccessResult<ProjectSettlementSummary>> GetAsync(
        Guid projectId, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.CompanyId is not { } companyId
            || currentUser.UserId is not { } userId || currentUser.Role is not { } role)
        {
            return AccessResult<ProjectSettlementSummary>.Failure(AccessResultStatus.Unauthorized);
        }

        // All categories, active membership and assignment see the same committed snapshot.
        // The non-relational path exists solely for isolated provider-backed tests.
        await using var transaction = db.Database.IsRelational()
            ? await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken) : null;
        if (transaction is not null)
        {
            await db.Database.ExecuteSqlRawAsync("SET TRANSACTION READ ONLY", cancellationToken);
        }

        var callerRole = await db.AppUsers.AsNoTracking()
            .Where(u => u.CompanyId == companyId && u.UserId == userId && u.Status == IdentityConstants.ActiveStatus
                && u.Company.CompanyId == companyId && u.Company.Status == IdentityConstants.ActiveStatus)
            .Select(u => u.Role).SingleOrDefaultAsync(cancellationToken);
        if (callerRole is null || callerRole != role)
        {
            return AccessResult<ProjectSettlementSummary>.Failure(AccessResultStatus.Unauthorized);
        }
        var capabilities = ProjectRoleCapabilities.For(role);
        if (!capabilities.CanViewAllCompanyProjects && !capabilities.RequiresSupervisorAssignment)
        {
            return AccessResult<ProjectSettlementSummary>.Failure(AccessResultStatus.Forbidden);
        }
        var restricted = capabilities.RequiresSupervisorAssignment;
        var project = await db.Projects.AsNoTracking().Where(p => p.CompanyId == companyId && p.ProjectId == projectId
                && (!restricted || db.ProjectSupervisors.Any(a => a.CompanyId == companyId && a.ProjectId == p.ProjectId
                    && a.SupervisorUserId == userId && a.IsActive)))
            .Select(p => new { p.Status, p.VersionNumber, p.CompletedAt, p.ActualEndDate })
            .SingleOrDefaultAsync(cancellationToken);
        if (project is null)
        {
            return AccessResult<ProjectSettlementSummary>.Failure(AccessResultStatus.NotFound);
        }

        var settings = await db.CompanySettings.AsNoTracking().Where(s => s.CompanyId == companyId && s.IsActive)
            .Select(s => new { s.ExpenseDocumentMode, s.ExpenseDocumentThresholdAmount })
            .SingleOrDefaultAsync(cancellationToken);
        var documentBoundary = ExpenseDocumentRules.RequiredFromAmount(
            settings?.ExpenseDocumentMode, settings?.ExpenseDocumentThresholdAmount);
        var categories = new List<SettlementCategorySummary>();
        var gaps = new List<SettlementEvaluationGap>
        {
            new("Advances", "ProjectAdvanceAttributionUnavailable"),
            new("ProjectLifecycle", "ProjectSettlementPolicyUndefined"),
            new("ProjectLifecycle", "ProjectClosurePolicyUndefined")
        };
        categories.Add(SettlementRules.Category("Advances", "None", [], "NotAttributable"));
        foreach (var query in ProjectSettlementQueries.Create(db, companyId, projectId, restricted, documentBoundary))
        {
            var records = await query.Records.AsNoTracking().ToArrayAsync(cancellationToken);
            categories.Add(SettlementRules.Category(query.Category, query.AmountMeaning, records));
            if (records.Any(r => r.Code.StartsWith("Unknown", StringComparison.Ordinal)
                || r.Code == "OutstandingAmountUnavailable"))
            {
                gaps.Add(new(query.Category, "FinancialRecordRequiresReview"));
            }
        }
        if (settings is null)
        {
            gaps.Add(new("ExpenseDocuments", "ActiveDocumentSettingsUnavailable"));
        }
        else if (settings.ExpenseDocumentMode is not ("Always" or "Threshold" or "Never")
            || settings.ExpenseDocumentMode == "Threshold" && settings.ExpenseDocumentThresholdAmount is null)
        {
            gaps.Add(new("ExpenseDocuments", "DocumentPolicyUnavailable"));
        }
        if (project.Status is "Completed" or "FinanciallyClosed"
            && (project.CompletedAt is null || project.ActualEndDate is null))
        {
            gaps.Add(new("ProjectLifecycle", "ProjectCompletionEvidenceUnavailable"));
        }
        if (restricted)
        {
            foreach (var category in new[] { "Reimbursements", "ManagerContributions", "SupplierPayments",
                "SupplierCredits", "ReimbursementPayments", "ExpenseReturns", "SupplierRefunds", "OwnerOperations" })
            {
                categories.Add(SettlementRules.Category(category, "None", [], "NotVisible"));
                gaps.Add(new(category, "FinancialCategoryNotVisible"));
            }
        }
        else
        {
            gaps.Add(new("SupplierCredits", "UnallocatedCreditProjectIntentUnavailable"));
            if (await db.ExpenseReturns.AsNoTracking().AnyAsync(r => r.CompanyId == companyId && r.Status == "Approved"
                && db.Expenses.Any(e => e.CompanyId == companyId && e.ProjectId == projectId && e.ExpenseId == r.ExpenseId),
                cancellationToken))
            {
                gaps.Add(new("ExpenseReturns", "ApprovedReturnEffectsRequireReconciliation"));
            }
            // Owner operations have no published non-Manager financial visibility policy.
            // Never expose their existence or amounts to other roles through this endpoint.
            if (role == IdentityConstants.ManagerRole)
            {
                var ownerRefunds = await db.OwnerPaymentRefunds.AsNoTracking()
                    .Where(r => r.CompanyId == companyId && r.ProjectId == projectId && r.Status == "PendingApproval")
                    .Select(r => new ProjectSettlementBlocker("OwnerOperations", "PendingOwnerRefund",
                        "OwnerPaymentRefund", r.OwnerPaymentRefundId, r.Status, null, null, null))
                    .ToArrayAsync(cancellationToken);
                var contractChanges = await db.ProjectContractChanges.AsNoTracking()
                    .Where(c => c.CompanyId == companyId && c.ProjectId == projectId && c.Status == "PendingApproval")
                    .Select(c => new ProjectSettlementBlocker("OwnerOperations", "PendingProjectContractChange",
                        "ProjectContractChange", c.ProjectContractChangeId, c.Status, null, null, null))
                    .ToArrayAsync(cancellationToken);
                categories.Add(SettlementRules.Category("OwnerOperations", "None", ownerRefunds.Concat(contractChanges)));
            }
            else
            {
                categories.Add(SettlementRules.Category("OwnerOperations", "None", [], "NotVisible"));
                gaps.Add(new("OwnerOperations", "FinancialCategoryNotVisible"));
            }
        }
        var result = SettlementRules.Summarize(projectId, project.Status, project.VersionNumber,
            timeProvider.GetUtcNow().UtcDateTime, restricted, categories, gaps);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }
        return AccessResult<ProjectSettlementSummary>.Success(result);
    }
}
