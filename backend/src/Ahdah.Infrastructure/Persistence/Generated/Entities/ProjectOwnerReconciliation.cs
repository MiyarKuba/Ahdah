using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectOwnerReconciliation
{
    public Guid ProjectOwnerReconciliationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public int ReconciliationNumber { get; set; }

    public long FinancialRevision { get; set; }

    public Guid PolicyVersionId { get; set; }

    public string Status { get; set; } = null!;

    public Guid PreparedByUserId { get; set; }

    public DateTime PreparedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid EvidenceId { get; set; }

    public Guid? SupersedesId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<ProjectOwnerReconciliation> InverseProjectOwnerReconciliationNavigation { get; set; } = new List<ProjectOwnerReconciliation>();

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectOwnerReconciliation? ProjectOwnerReconciliationNavigation { get; set; }

    public virtual ICollection<ProjectOwnerReconciliationPosition> ProjectOwnerReconciliationPositions { get; set; } = new List<ProjectOwnerReconciliationPosition>();

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;

    public virtual VerifiedEvidenceObject VerifiedEvidenceObject { get; set; } = null!;
}
