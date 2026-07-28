using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class PersonalClaimPaymentAllocation
{
    public Guid PersonalClaimPaymentAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid PersonalClaimPaymentId { get; set; }

    public Guid PersonalClaimId { get; set; }

    public int ClaimVersionNumber { get; set; }

    public decimal AllocatedAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual PersonalClaim PersonalClaim { get; set; } = null!;

    public virtual PersonalClaimPayment PersonalClaimPayment { get; set; } = null!;
}
