using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierPaymentDebtAllocation
{
    public Guid SupplierPaymentDebtAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SupplierPaymentId { get; set; }

    public Guid SupplierDebtId { get; set; }

    public int DebtVersionNumber { get; set; }

    public decimal AllocatedAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual SupplierDebt SupplierDebt { get; set; } = null!;

    public virtual SupplierPayment SupplierPayment { get; set; } = null!;
}
