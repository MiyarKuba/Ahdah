using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierRefund
{
    public Guid SupplierRefundId { get; set; }

    public Guid CompanyId { get; set; }

    public string RefundNumber { get; set; } = null!;

    public Guid ExpenseReturnId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid FundingSourceId { get; set; }

    public DateOnly RefundDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal RefundAmount { get; set; }

    public decimal FeeAmount { get; set; }

    public decimal? NetReceivedAmount { get; set; }

    public string RefundMethod { get; set; } = null!;

    public string? SenderBankName { get; set; }

    public string? SupplierReferenceNumber { get; set; }

    public string? TransactionReferenceNumber { get; set; }

    public string? ProofFileUrl { get; set; }

    public string Description { get; set; } = null!;

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid RecordedByUserId { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

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

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual ExpenseReturn ExpenseReturn { get; set; } = null!;

    public virtual FundingSource FundingSource { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
