using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectSettlementSnapshotCategory
{
    public Guid ProjectSettlementSnapshotCategoryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid SnapshotId { get; set; }

    public string CategoryCode { get; set; } = null!;

    public string EvaluationStatus { get; set; } = null!;

    public string? CurrencyCode { get; set; }

    public string AmountMeaning { get; set; } = null!;

    public decimal? ExactAmount { get; set; }

    public int BlockerCount { get; set; }

    public int GapCount { get; set; }

    public string Details { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectSettlementSnapshot ProjectSettlementSnapshot { get; set; } = null!;
}
