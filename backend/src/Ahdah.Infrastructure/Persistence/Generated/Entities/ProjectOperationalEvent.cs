using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectOperationalEvent
{
    public Guid ProjectOperationalEventId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public string EventType { get; set; } = null!;

    public Guid ActorUserId { get; set; }

    public string ActorRole { get; set; } = null!;

    public DateTime OccurredAt { get; set; }

    public string Reason { get; set; } = null!;

    public int ProjectVersionBefore { get; set; }

    public int ProjectVersionAfter { get; set; }

    public Guid? EvidenceId { get; set; }

    public Guid? AcknowledgmentEventId { get; set; }

    public Guid AuthorizationDecisionId { get; set; }

    public Guid IdempotencyRecordId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual IdempotencyRecord IdempotencyRecord { get; set; } = null!;

    public virtual ICollection<ProjectOperationalEvent> InverseProjectOperationalEventNavigation { get; set; } = new List<ProjectOperationalEvent>();

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision ProjectAuthorizationDecision { get; set; } = null!;

    public virtual ProjectOperationalEvent? ProjectOperationalEventNavigation { get; set; }

    public virtual VerifiedEvidenceObject? VerifiedEvidenceObject { get; set; }
}
