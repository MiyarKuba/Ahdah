using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class NotificationDelivery
{
    public Guid NotificationDeliveryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid NotificationId { get; set; }

    public Guid RecipientUserId { get; set; }

    public int NotificationVersionNumber { get; set; }

    public string DeliveryChannel { get; set; } = null!;

    public string DeliveryTargetType { get; set; } = null!;

    public string? DestinationSnapshot { get; set; }

    public string? TargetIdentifierHash { get; set; }

    public string TitleSnapshot { get; set; } = null!;

    public string MessageSnapshot { get; set; } = null!;

    public string PayloadSnapshot { get; set; } = null!;

    public string? ProviderName { get; set; }

    public string? ProviderMessageId { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string DeliveryStatus { get; set; } = null!;

    public int AttemptCount { get; set; }

    public int MaxAttempts { get; set; }

    public DateTime? QueuedAt { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? LastFailedAt { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? SkipReason { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public string ProviderMetadata { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public bool? IsFinal { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Notification Notification { get; set; } = null!;
}
