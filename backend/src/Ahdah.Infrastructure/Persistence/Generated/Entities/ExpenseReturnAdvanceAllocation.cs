using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ExpenseReturnAdvanceAllocation
{
    public Guid ExpenseReturnAdvanceAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ExpenseReturnId { get; set; }

    public Guid ExpenseAdvanceAllocationId { get; set; }

    public int BalanceVersionNumber { get; set; }

    public decimal RestoredAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual ExpenseAdvanceAllocation ExpenseAdvanceAllocation { get; set; } = null!;

    public virtual ExpenseReturn ExpenseReturn { get; set; } = null!;
}
