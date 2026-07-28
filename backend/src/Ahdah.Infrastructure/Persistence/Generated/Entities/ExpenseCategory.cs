using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ExpenseCategory
{
    public Guid ExpenseCategoryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? ParentExpenseCategoryId { get; set; }

    public string? CategoryCode { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CategoryGroup { get; set; } = null!;

    public string ExpenseScope { get; set; } = null!;

    public string? Description { get; set; }

    public bool RequiresSupplier { get; set; }

    public bool RequiresReceipt { get; set; }

    public bool SupportsQuantityDetails { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? DeactivatedByUserId { get; set; }

    public DateTime? DeactivatedAt { get; set; }

    public string? DeactivationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ExpenseCategory? ExpenseCategoryNavigation { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<ExpenseCategory> InverseExpenseCategoryNavigation { get; set; } = new List<ExpenseCategory>();
}
