using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectFinancialControl
{
    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public long FinancialRevision { get; set; }

    public string FinancialState { get; set; } = null!;

    public Guid? ActiveSettlementId { get; set; }

    public Guid? ActiveClosureId { get; set; }

    public DateTime LastMaterialChangeAt { get; set; }

    public int NextCycleNumber { get; set; }

    public string AttributionStatus { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectClosureCertificate? ProjectClosureCertificate { get; set; }

    public virtual ProjectSettlement? ProjectSettlement { get; set; }
}
