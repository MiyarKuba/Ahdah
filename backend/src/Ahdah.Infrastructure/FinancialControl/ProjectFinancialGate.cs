using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.Infrastructure.FinancialControl;

public sealed class ProjectFinancialGate(AhdahDbContext dbContext, TimeProvider timeProvider)
{
    public async Task<ProjectFinancialControl?> GetAsync(
        Guid companyId, Guid projectId, CancellationToken cancellationToken) =>
        await dbContext.ProjectFinancialControls.AsNoTracking().SingleOrDefaultAsync(
            control => control.CompanyId == companyId && control.ProjectId == projectId,
            cancellationToken);

    public async Task<IReadOnlyList<ProjectFinancialControl>?> LockAsync(
        Guid companyId, IEnumerable<Guid> projectIds, CancellationToken cancellationToken)
    {
        var requested = projectIds.Distinct().ToArray();
        var controls = await dbContext.ProjectFinancialControls
            .FromSqlInterpolated($"SELECT * FROM ahdah.project_financial_controls WHERE company_id = {companyId} AND project_id = ANY ({requested}) ORDER BY project_id FOR UPDATE")
            .ToArrayAsync(cancellationToken);
        return controls.Length == requested.Length ? controls : null;
    }

    public static bool AllowsOrdinaryFinancialMutation(ProjectFinancialControl control) =>
        control.FinancialState != "FinanciallyClosed";

    public long AdvanceRevision(ProjectFinancialControl control)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        control.FinancialRevision++;
        control.VersionNumber++;
        control.LastMaterialChangeAt = now;
        control.UpdatedAt = now;
        return control.FinancialRevision;
    }
}
