using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectReopening
{
    public Guid ProjectReopeningId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid PreviousClosureId { get; set; }

    public Guid NewSettlementId { get; set; }

    public Guid ActorUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public DateTime ReopenedAt { get; set; }

    public string PriorFinancialState { get; set; } = null!;

    public string ResultingFinancialState { get; set; } = null!;

    public long RevisionBefore { get; set; }

    public long RevisionAfter { get; set; }

    public int PreProjectVersion { get; set; }

    public int PostProjectVersion { get; set; }

    public Guid AuthorizationDecisionId { get; set; }

    public Guid AuditLogId { get; set; }

    public Guid IdempotencyRecordId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AuditLog AuditLog { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual IdempotencyRecord IdempotencyRecord { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision ProjectAuthorizationDecision { get; set; } = null!;

    public virtual ProjectClosureCertificate ProjectClosureCertificate { get; set; } = null!;

    public virtual ProjectSettlement ProjectSettlement { get; set; } = null!;
}
