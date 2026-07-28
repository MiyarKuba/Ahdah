using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class OwnerPaymentRefund
{
    public Guid OwnerPaymentRefundId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectOwnerId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? ProjectOwnerPaymentId { get; set; }

    public string RefundNumber { get; set; } = null!;

    public DateOnly RefundDate { get; set; }

    public decimal RefundAmount { get; set; }

    public string RefundMethod { get; set; } = null!;

    public string? BankName { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? ProofFileUrl { get; set; }

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid RequestedByUserId { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

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

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual ICollection<OwnerRefundFundingSource> OwnerRefundFundingSources { get; set; } = new List<OwnerRefundFundingSource>();

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectOwner ProjectOwner { get; set; } = null!;

    public virtual ProjectOwnerPayment? ProjectOwnerPayment { get; set; }
}
