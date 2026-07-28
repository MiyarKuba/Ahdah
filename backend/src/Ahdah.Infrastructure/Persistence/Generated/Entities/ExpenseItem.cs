using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ExpenseItem
{
    public Guid ExpenseItemId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ExpenseId { get; set; }

    public int LineNumber { get; set; }

    public string ItemName { get; set; } = null!;

    public string? ItemCode { get; set; }

    public string? ItemDescription { get; set; }

    public decimal Quantity { get; set; }

    public string UnitCode { get; set; } = null!;

    public string? CustomUnitName { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? SubtotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Expense Expense { get; set; } = null!;

    public virtual ICollection<ExpenseReturnItem> ExpenseReturnItems { get; set; } = new List<ExpenseReturnItem>();
}
