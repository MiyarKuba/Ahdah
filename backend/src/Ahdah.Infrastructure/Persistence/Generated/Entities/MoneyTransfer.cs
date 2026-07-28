using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class MoneyTransfer
{
    public Guid MoneyTransferId { get; set; }

    public Guid CompanyId { get; set; }

    public string TransferNumber { get; set; } = null!;

    public string TransferType { get; set; } = null!;

    public Guid? AdvanceId { get; set; }

    public Guid SenderUserId { get; set; }

    public Guid RecipientUserId { get; set; }

    public decimal TransferAmount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public DateOnly TransferDate { get; set; }

    public string TransferMethod { get; set; } = null!;

    public string? BankName { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? ProofFileUrl { get; set; }

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid InitiatedByUserId { get; set; }

    public Guid? ConfirmedByUserId { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public Guid? RejectedByUserId { get; set; }

    public DateTime? RejectedAt { get; set; }

    public string? RejectionReason { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Guid? ReversedByUserId { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Advance? Advance { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser AppUser2 { get; set; } = null!;

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser? AppUser4 { get; set; }

    public virtual AppUser AppUser5 { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ICollection<TransferAdvanceAllocation> TransferAdvanceAllocations { get; set; } = new List<TransferAdvanceAllocation>();

    public virtual TransferCorrectionRequest? TransferCorrectionRequest { get; set; }
}
