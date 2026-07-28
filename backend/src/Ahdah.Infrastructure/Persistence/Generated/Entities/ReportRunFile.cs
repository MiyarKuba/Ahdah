using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportRunFile
{
    public Guid ReportRunFileId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportRunId { get; set; }

    public Guid? SourceReportRunFileId { get; set; }

    public int FileSequence { get; set; }

    public string ExportFormat { get; set; } = null!;

    public int PartNumber { get; set; }

    public int TotalParts { get; set; }

    public bool IsPrimary { get; set; }

    public string FileOrigin { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FileExtension { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string? StorageProvider { get; set; }

    public string? StorageContainer { get; set; }

    public string? StorageKey { get; set; }

    public string? StorageRegion { get; set; }

    public string? StorageEtag { get; set; }

    public string? StorageVersionId { get; set; }

    public long? FileSizeBytes { get; set; }

    public string? Sha256Hash { get; set; }

    public bool IsEncrypted { get; set; }

    public string? EncryptionAlgorithm { get; set; }

    public string? EncryptionKeyReference { get; set; }

    public bool RequiresAuthorization { get; set; }

    public string FileStatus { get; set; } = null!;

    public DateTime? GenerationStartedAt { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public DateTime? AvailableAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? FailedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public long DownloadCount { get; set; }

    public DateTime? LastDownloadedAt { get; set; }

    public Guid? LastDownloadedByUserId { get; set; }

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureDetails { get; set; }

    public string? DeletedByType { get; set; }

    public Guid? DeletedByUserId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletionReason { get; set; }

    public Guid CorrelationId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ICollection<ReportRunFile> InverseReportRunFileNavigation { get; set; } = new List<ReportRunFile>();

    public virtual ICollection<ReportDeliveryFile> ReportDeliveryFiles { get; set; } = new List<ReportDeliveryFile>();

    public virtual ICollection<ReportFileDownload> ReportFileDownloads { get; set; } = new List<ReportFileDownload>();

    public virtual ReportRun ReportRun { get; set; } = null!;

    public virtual ReportRunFile? ReportRunFileNavigation { get; set; }
}
