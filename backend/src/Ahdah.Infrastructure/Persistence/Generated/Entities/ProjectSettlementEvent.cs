using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectSettlementEvent
{
    public Guid ProjectSettlementEventId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid SettlementId { get; set; }

    public int EventNumber { get; set; }

    public string EventType { get; set; } = null!;

    public Guid ActorUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public DateTime OccurredAt { get; set; }

    public string? Reason { get; set; }

    public Guid? AuthorizationDecisionId { get; set; }

    public Guid? SnapshotId { get; set; }

    public long FinancialRevision { get; set; }

    public int RecordVersionBefore { get; set; }

    public int RecordVersionAfter { get; set; }

    public Guid IdempotencyRecordId { get; set; }

    public string Payload { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual IdempotencyRecord IdempotencyRecord { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision? ProjectAuthorizationDecision { get; set; }

    public virtual ProjectSettlement ProjectSettlement { get; set; } = null!;

    public virtual ProjectSettlementSnapshot? ProjectSettlementSnapshot { get; set; }
}
