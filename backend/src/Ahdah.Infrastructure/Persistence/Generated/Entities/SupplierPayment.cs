using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierPayment
{
    public Guid SupplierPaymentId { get; set; }

    public Guid CompanyId { get; set; }

    public string PaymentNumber { get; set; } = null!;

    public Guid SupplierId { get; set; }

    public Guid? SupplierPaymentAccountId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal PaymentAmount { get; set; }

    public decimal FeeAmount { get; set; }

    public decimal? TotalDisbursedAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PayerBankName { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? ProofFileUrl { get; set; }

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

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

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser? AppUser4 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual SupplierPaymentAccount? SupplierPaymentAccount { get; set; }

    public virtual ICollection<SupplierPaymentDebtAllocation> SupplierPaymentDebtAllocations { get; set; } = new List<SupplierPaymentDebtAllocation>();

    public virtual ICollection<SupplierPaymentFundingSource> SupplierPaymentFundingSources { get; set; } = new List<SupplierPaymentFundingSource>();
}
