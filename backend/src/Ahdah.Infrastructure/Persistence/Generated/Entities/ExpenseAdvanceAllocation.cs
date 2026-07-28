using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ExpenseAdvanceAllocation
{
    public Guid ExpenseAdvanceAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ExpenseId { get; set; }

    public Guid UserAdvanceBalanceId { get; set; }

    public decimal AllocatedAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Expense Expense { get; set; } = null!;

    public virtual ICollection<ExpenseReturnAdvanceAllocation> ExpenseReturnAdvanceAllocations { get; set; } = new List<ExpenseReturnAdvanceAllocation>();

    public virtual UserAdvanceBalance UserAdvanceBalance { get; set; } = null!;
}
