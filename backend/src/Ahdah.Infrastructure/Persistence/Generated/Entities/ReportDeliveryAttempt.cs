using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportDeliveryAttempt
{
    public Guid ReportDeliveryAttemptId { get; set; }

    public long AttemptSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportDeliveryId { get; set; }

    public Guid BackgroundJobId { get; set; }

    public Guid BackgroundJobAttemptId { get; set; }

    public int AttemptNumber { get; set; }

    public string DeliveryChannelSnapshot { get; set; } = null!;

    public string RecipientTypeSnapshot { get; set; } = null!;

    public Guid? RecipientUserIdSnapshot { get; set; }

    public string? RecipientEmailSnapshot { get; set; }

    public string? RecipientDisplayNameSnapshot { get; set; }

    public string DestinationIdentifierSnapshot { get; set; } = null!;

    public string ProviderName { get; set; } = null!;

    public string? ProviderEndpoint { get; set; }

    public string? ProviderRequestId { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? ProviderStatusCode { get; set; }

    public string? ProviderStatusMessage { get; set; }

    public bool WasProviderContacted { get; set; }

    public bool ProviderResponseReceived { get; set; }

    public short? HttpStatusCode { get; set; }

    public string? RequestedPayloadHash { get; set; }

    public string? ResponsePayloadHash { get; set; }

    public string RequestMetadata { get; set; } = null!;

    public string ResponseMetadata { get; set; } = null!;

    public int DownloadLinkCountSnapshot { get; set; }

    public int AttachmentCountSnapshot { get; set; }

    public long AttachmentBytesSnapshot { get; set; }

    public string AttemptStatus { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? ProviderStartedAt { get; set; }

    public DateTime? ProviderCompletedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    public long DurationMilliseconds { get; set; }

    public long? ProviderDurationMilliseconds { get; set; }

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureDetails { get; set; }

    public string RetryDecision { get; set; } = null!;

    public DateTime? NextRetryAt { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual BackgroundJobAttempt BackgroundJobAttempt { get; set; } = null!;

    public virtual ReportDelivery ReportDelivery { get; set; } = null!;

    public virtual ICollection<ReportDeliveryEvent> ReportDeliveryEvents { get; set; } = new List<ReportDeliveryEvent>();
}
