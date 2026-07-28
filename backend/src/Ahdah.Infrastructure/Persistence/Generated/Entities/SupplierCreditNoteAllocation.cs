using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierCreditNoteAllocation
{
    public Guid SupplierCreditNoteAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SupplierCreditNoteId { get; set; }

    public Guid SupplierDebtId { get; set; }

    public int DebtVersionNumber { get; set; }

    public decimal AllocatedAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual SupplierCreditNote SupplierCreditNote { get; set; } = null!;

    public virtual SupplierDebt SupplierDebt { get; set; } = null!;
}
