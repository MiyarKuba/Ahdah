using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectClosureCertificate
{
    public Guid ProjectClosureCertificateId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid ProjectSettlementId { get; set; }

    public Guid ApprovedSnapshotId { get; set; }

    public Guid ClosureSnapshotId { get; set; }

    public long FinancialRevision { get; set; }

    public Guid PolicyVersionId { get; set; }

    public string ClosureBasis { get; set; } = null!;

    public Guid ClosedByUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public DateTime ClosedAt { get; set; }

    public string Reason { get; set; } = null!;

    public int PreCloseProjectVersion { get; set; }

    public int PostCloseProjectVersion { get; set; }

    public byte[] ApprovedSnapshotHash { get; set; } = null!;

    public byte[] ClosureSnapshotHash { get; set; } = null!;

    public int ClosureVersion { get; set; }

    public Guid AuthorizationDecisionId { get; set; }

    public Guid IdempotencyRecordId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual IdempotencyRecord IdempotencyRecord { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision ProjectAuthorizationDecision { get; set; } = null!;

    public virtual ICollection<ProjectFinancialControl> ProjectFinancialControls { get; set; } = new List<ProjectFinancialControl>();

    public virtual ICollection<ProjectReopening> ProjectReopenings { get; set; } = new List<ProjectReopening>();

    public virtual ProjectSettlement ProjectSettlement { get; set; } = null!;

    public virtual ProjectSettlementSnapshot ProjectSettlementSnapshot { get; set; } = null!;

    public virtual ProjectSettlementSnapshot ProjectSettlementSnapshot1 { get; set; } = null!;

    public virtual ProjectSettlementSnapshot ProjectSettlementSnapshot2 { get; set; } = null!;

    public virtual ProjectSettlementSnapshot ProjectSettlementSnapshotNavigation { get; set; } = null!;

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;
}
