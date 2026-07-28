using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class OwnerPaymentProjectAllocation
{
    public Guid OwnerPaymentProjectAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectOwnerPaymentId { get; set; }

    public Guid ProjectId { get; set; }

    public decimal AllocatedAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectOwnerPayment ProjectOwnerPayment { get; set; } = null!;
}
