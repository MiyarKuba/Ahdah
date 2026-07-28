using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectOwnerPayment
{
    public Guid ProjectOwnerPaymentId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid FundingSourceId { get; set; }

    public Guid ProjectOwnerId { get; set; }

    public string PaymentNumber { get; set; } = null!;

    public DateOnly PaymentDate { get; set; }

    public decimal PaymentAmount { get; set; }

    public string Status { get; set; } = null!;

    public Guid RecordedByUserId { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? Notes { get; set; }

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

    public virtual FundingSource FundingSource { get; set; } = null!;

    public virtual ICollection<OwnerPaymentProjectAllocation> OwnerPaymentProjectAllocations { get; set; } = new List<OwnerPaymentProjectAllocation>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefunds { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ProjectOwner ProjectOwner { get; set; } = null!;
}
