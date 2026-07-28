using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid RecipientUserId { get; set; }

    public string NotificationCategory { get; set; } = null!;

    public string NotificationCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public string LocaleCode { get; set; } = null!;

    public string? EntityType { get; set; }

    public Guid? EntityId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? ActionTarget { get; set; }

    public string Payload { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public string? DeduplicationKey { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? ScheduledAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? SeenAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime? ArchivedAt { get; set; }

    public string CreatedByType { get; set; } = null!;

    public Guid? CreatedByUserId { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual NotificationDelivery? NotificationDelivery { get; set; }
}
