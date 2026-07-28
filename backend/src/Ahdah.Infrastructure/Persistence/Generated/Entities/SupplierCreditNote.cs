using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierCreditNote
{
    public Guid SupplierCreditNoteId { get; set; }

    public Guid CompanyId { get; set; }

    public string CreditNoteNumber { get; set; } = null!;

    public Guid SupplierId { get; set; }

    public string? SupplierReferenceNumber { get; set; }

    public DateOnly CreditNoteDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal CreditNoteAmount { get; set; }

    public string ReasonType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ProofFileUrl { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

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

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser? AppUser4 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<SupplierCreditNoteAllocation> SupplierCreditNoteAllocations { get; set; } = new List<SupplierCreditNoteAllocation>();
}
