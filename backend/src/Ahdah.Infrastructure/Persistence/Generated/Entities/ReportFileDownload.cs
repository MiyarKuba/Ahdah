using System;
using System.Collections.Generic;
using System.Net;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportFileDownload
{
    public Guid ReportFileDownloadId { get; set; }

    public long DownloadSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportRunFileId { get; set; }

    public string RequestedByType { get; set; } = null!;

    public Guid? RequestedByUserId { get; set; }

    public string AccessChannel { get; set; } = null!;

    public string AuthenticationMethod { get; set; } = null!;

    public string? ClientApplication { get; set; }

    public string? SessionIdentifier { get; set; }

    public string? DeviceIdentifier { get; set; }

    public IPAddress? ClientIp { get; set; }

    public string? ForwardedFor { get; set; }

    public string? UserAgent { get; set; }

    public string FileNameSnapshot { get; set; } = null!;

    public string ExportFormatSnapshot { get; set; } = null!;

    public string ContentTypeSnapshot { get; set; } = null!;

    public long FileSizeBytesSnapshot { get; set; }

    public string Sha256HashSnapshot { get; set; } = null!;

    public bool RequiresAuthorizationSnapshot { get; set; }

    public bool IsEncryptedSnapshot { get; set; }

    public string AuthorizationDecision { get; set; } = null!;

    public string? AuthorizationPolicyCode { get; set; }

    public string DownloadStatus { get; set; } = null!;

    public short? HttpStatusCode { get; set; }

    public string AccessLinkType { get; set; } = null!;

    public string? TemporaryUrlTokenHash { get; set; }

    public DateTime? TemporaryUrlIssuedAt { get; set; }

    public DateTime? TemporaryUrlExpiresAt { get; set; }

    public bool IsPartialDownload { get; set; }

    public long? RangeStartByte { get; set; }

    public long? RangeEndByte { get; set; }

    public long BytesTransferred { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? AuthorizedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    public long DurationMilliseconds { get; set; }

    public string? DenialCategory { get; set; }

    public string? DenialCode { get; set; }

    public string? DenialMessage { get; set; }

    public string? FailureCategory { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureDetails { get; set; }

    public string? CancellationReason { get; set; }

    public string? DownloadPurpose { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual ReportRunFile ReportRunFile { get; set; } = null!;
}
