using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectOwnerReconciliationPosition
{
    public Guid ProjectOwnerReconciliationPositionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid OwnerReconciliationId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal? ContractAmount { get; set; }

    public decimal? ApprovedChangeAmount { get; set; }

    public decimal? PaymentsReceivedAmount { get; set; }

    public decimal? RefundsAmount { get; set; }

    public decimal? ImmediateReceivableAmount { get; set; }

    public decimal? ImmediatePayableAmount { get; set; }

    public decimal? RetentionAmount { get; set; }

    public decimal? DisputedAmount { get; set; }

    public decimal? UnclassifiedDifferenceAmount { get; set; }

    public string ClassificationStatus { get; set; } = null!;

    public string SourceDetails { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectOwnerReconciliation ProjectOwnerReconciliation { get; set; } = null!;

    public virtual ICollection<ProjectOwnerRetentionItem> ProjectOwnerRetentionItems { get; set; } = new List<ProjectOwnerRetentionItem>();
}
