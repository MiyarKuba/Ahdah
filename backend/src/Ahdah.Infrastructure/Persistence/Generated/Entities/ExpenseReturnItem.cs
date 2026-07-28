using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ExpenseReturnItem
{
    public Guid ExpenseReturnItemId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ExpenseReturnId { get; set; }

    public Guid ExpenseItemId { get; set; }

    public int LineNumber { get; set; }

    public string ItemNameSnapshot { get; set; } = null!;

    public decimal ReturnedQuantity { get; set; }

    public string UnitCodeSnapshot { get; set; } = null!;

    public string? CustomUnitNameSnapshot { get; set; }

    public decimal UnitReturnPrice { get; set; }

    public decimal? SubtotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal? TotalReturnAmount { get; set; }

    public string? ItemCondition { get; set; }

    public bool IsRestockable { get; set; }

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual ExpenseItem ExpenseItem { get; set; } = null!;

    public virtual ExpenseReturn ExpenseReturn { get; set; } = null!;
}
