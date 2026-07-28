using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class AdvanceClosureDocument
{
    public Guid AdvanceClosureDocumentId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid AdvanceClosureId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string DocumentTitle { get; set; } = null!;

    public string? DocumentNumber { get; set; }

    public DateOnly? DocumentDate { get; set; }

    public string? IssuerName { get; set; }

    public string OriginalFileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public string Sha256Hash { get; set; } = null!;

    public string CaptureSource { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public bool IsRequired { get; set; }

    public string VerificationStatus { get; set; } = null!;

    public Guid UploadedByUserId { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public string? Notes { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AdvanceClosure AdvanceClosure { get; set; } = null!;

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }
}
