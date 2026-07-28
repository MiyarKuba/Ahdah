using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Expense
{
    public Guid ExpenseId { get; set; }

    public Guid CompanyId { get; set; }

    public string ExpenseNumber { get; set; } = null!;

    public Guid? ProjectId { get; set; }

    public Guid ExpenseCategoryId { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid IncurredByUserId { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public string PaymentMode { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public decimal SubtotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public DateOnly? CreditDueDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public string? ReceiptNumber { get; set; }

    public string? MerchantName { get; set; }

    public string Description { get; set; } = null!;

    public string? ExpenseLocation { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? CorrectionReason { get; set; }

    public string? RejectionReason { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Guid? ReversedByUserId { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUser3 { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual ICollection<ExpenseAdvanceAllocation> ExpenseAdvanceAllocations { get; set; } = new List<ExpenseAdvanceAllocation>();

    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;

    public virtual ExpenseDocument? ExpenseDocument { get; set; }

    public virtual ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();

    public virtual ICollection<ExpenseReturn> ExpenseReturns { get; set; } = new List<ExpenseReturn>();

    public virtual PersonalClaim? PersonalClaim { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public virtual SupplierDebt? SupplierDebt { get; set; }
}
