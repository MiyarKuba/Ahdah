using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class TransferCorrectionRequest
{
    public Guid TransferCorrectionRequestId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid MoneyTransferId { get; set; }

    public Guid RequestedByUserId { get; set; }

    public int TransferVersionNumber { get; set; }

    public string CorrectionType { get; set; } = null!;

    public string RequestReason { get; set; } = null!;

    public string RequestedChanges { get; set; } = null!;

    public string Status { get; set; } = null!;

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewNotes { get; set; }

    public string? RejectionReason { get; set; }

    public Guid? AppliedByUserId { get; set; }

    public DateTime? AppliedAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual MoneyTransfer MoneyTransfer { get; set; } = null!;
}
