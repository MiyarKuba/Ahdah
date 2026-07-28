using System;
using System.Collections.Generic;
using System.Net;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportDeliveryEvent
{
    public Guid ReportDeliveryEventId { get; set; }

    public long EventSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportDeliveryId { get; set; }

    public Guid ReportDeliveryAttemptId { get; set; }

    public Guid? DuplicateOfEventId { get; set; }

    public string DeliveryChannelSnapshot { get; set; } = null!;

    public string RecipientTypeSnapshot { get; set; } = null!;

    public Guid? RecipientUserIdSnapshot { get; set; }

    public string? RecipientEmailSnapshot { get; set; }

    public string? RecipientDisplayNameSnapshot { get; set; }

    public string ProviderName { get; set; } = null!;

    public string? ProviderMessageId { get; set; }

    public string? ProviderEventId { get; set; }

    public string? WebhookRequestId { get; set; }

    public string EventSource { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string EventCategory { get; set; } = null!;

    public string ProcessingResult { get; set; } = null!;

    public string SignatureVerificationStatus { get; set; } = null!;

    public DateTime OccurredAt { get; set; }

    public DateTime ReceivedAt { get; set; }

    public DateTime ProcessedAt { get; set; }

    public string? ProviderStatusCode { get; set; }

    public string? ProviderStatusMessage { get; set; }

    public string? EventReasonCode { get; set; }

    public string? EventReasonMessage { get; set; }

    public string? BounceType { get; set; }

    public string? SmtpResponseCode { get; set; }

    public string? ClickedUrlHash { get; set; }

    public IPAddress? ClientIp { get; set; }

    public string? UserAgent { get; set; }

    public string PayloadHash { get; set; } = null!;

    public string EventMetadata { get; set; } = null!;

    public string IdempotencyKey { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual ICollection<ReportDeliveryEvent> InverseReportDeliveryEventNavigation { get; set; } = new List<ReportDeliveryEvent>();

    public virtual ReportDeliveryAttempt ReportDeliveryAttempt { get; set; } = null!;

    public virtual ReportDeliveryEvent? ReportDeliveryEventNavigation { get; set; }

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEvents { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressions { get; set; } = new List<ReportRecipientSuppression>();
}
