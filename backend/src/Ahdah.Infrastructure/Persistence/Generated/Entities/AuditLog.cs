using System;
using System.Collections.Generic;
using System.Net;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class AuditLog
{
    public Guid AuditLogId { get; set; }

    public long AuditSequence { get; set; }

    public Guid CompanyId { get; set; }

    public string EventCategory { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public string EventAction { get; set; } = null!;

    public string Severity { get; set; } = null!;

    public string Outcome { get; set; } = null!;

    public string ActorType { get; set; } = null!;

    public Guid? ActorUserId { get; set; }

    public string ActorNameSnapshot { get; set; } = null!;

    public string? ActorRoleSnapshot { get; set; }

    public string? EntityType { get; set; }

    public Guid? EntityId { get; set; }

    public int? EntityVersionNumber { get; set; }

    public string Description { get; set; } = null!;

    public string? BeforeValues { get; set; }

    public string? AfterValues { get; set; }

    public string ChangedFields { get; set; } = null!;

    public string Metadata { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public Guid? SessionId { get; set; }

    public string? RequestId { get; set; }

    public string? IdempotencyKey { get; set; }

    public string SourceType { get; set; } = null!;

    public string? RequestMethod { get; set; }

    public string? RequestPath { get; set; }

    public int? HttpStatusCode { get; set; }

    public IPAddress? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceIdentifierHash { get; set; }

    public string? ClientAppVersion { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual Company Company { get; set; } = null!;
}
