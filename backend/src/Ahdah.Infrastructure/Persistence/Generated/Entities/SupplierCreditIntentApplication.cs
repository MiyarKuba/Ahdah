using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierCreditIntentApplication
{
    public Guid SupplierCreditIntentApplicationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SupplierCreditIntentId { get; set; }

    public Guid SupplierCreditNoteAllocationId { get; set; }

    public decimal Amount { get; set; }

    public Guid AppliedByUserId { get; set; }

    public DateTime AppliedAt { get; set; }

    public Guid? ReversesApplicationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual SupplierCreditIntentApplication? InverseSupplierCreditIntentApplicationNavigation { get; set; }

    public virtual SupplierCreditIntent SupplierCreditIntent { get; set; } = null!;

    public virtual SupplierCreditIntentApplication? SupplierCreditIntentApplicationNavigation { get; set; }

    public virtual SupplierCreditNoteAllocation SupplierCreditNoteAllocation { get; set; } = null!;
}
