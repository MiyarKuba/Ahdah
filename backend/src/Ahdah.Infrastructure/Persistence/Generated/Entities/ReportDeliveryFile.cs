using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportDeliveryFile
{
    public Guid ReportDeliveryFileId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportRunId { get; set; }

    public Guid ReportDeliveryId { get; set; }

    public Guid ReportRunFileId { get; set; }

    public int FileSequence { get; set; }

    public string DeliveryFileUsage { get; set; } = null!;

    public string FileNameSnapshot { get; set; } = null!;

    public string ExportFormatSnapshot { get; set; } = null!;

    public string ContentTypeSnapshot { get; set; } = null!;

    public long FileSizeBytesSnapshot { get; set; }

    public string Sha256HashSnapshot { get; set; } = null!;

    public DateTime SourceFileExpiresAtSnapshot { get; set; }

    public bool SourceFileEncryptedSnapshot { get; set; }

    public bool RequiresAuthorizationSnapshot { get; set; }

    public string? TemporaryUrlTokenHash { get; set; }

    public DateTime? TemporaryUrlIssuedAt { get; set; }

    public DateTime? TemporaryUrlExpiresAt { get; set; }

    public int? MaximumDownloadCount { get; set; }

    public int SuccessfulDownloadCount { get; set; }

    public DateTime? LastSuccessfulDownloadAt { get; set; }

    public string? ProviderAttachmentId { get; set; }

    public string ProviderMetadata { get; set; } = null!;

    public string IdempotencyKey { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? PreparedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public DateTime? SkippedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public string? RevokedByType { get; set; }

    public Guid? RevokedByUserId { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevocationReason { get; set; }

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureDetails { get; set; }

    public string? SkipCategory { get; set; }

    public string? SkipReason { get; set; }

    public Guid CorrelationId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual ReportDelivery ReportDelivery { get; set; } = null!;

    public virtual ReportRunFile ReportRunFile { get; set; } = null!;
}
