using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectOwnerRetentionItem
{
    public Guid ProjectOwnerRetentionItemId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid OwnerPositionId { get; set; }

    public decimal Amount { get; set; }

    public decimal? Percentage { get; set; }

    public decimal? PercentageBaseAmount { get; set; }

    public string ContractualBasis { get; set; } = null!;

    public string ReleaseCondition { get; set; } = null!;

    public DateOnly? DueDate { get; set; }

    public string EntitlementStatus { get; set; } = null!;

    public string Classification { get; set; } = null!;

    public Guid EvidenceId { get; set; }

    public Guid ApprovedByUserId { get; set; }

    public DateTime ApprovedAt { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectOwnerReconciliationPosition ProjectOwnerReconciliationPosition { get; set; } = null!;

    public virtual VerifiedEvidenceObject VerifiedEvidenceObject { get; set; } = null!;
}
