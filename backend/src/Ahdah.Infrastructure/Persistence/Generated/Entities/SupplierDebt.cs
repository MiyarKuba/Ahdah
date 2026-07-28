using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierDebt
{
    public Guid SupplierDebtId { get; set; }

    public Guid CompanyId { get; set; }

    public string DebtNumber { get; set; } = null!;

    public Guid SupplierId { get; set; }

    public Guid ExpenseId { get; set; }

    public Guid? ProjectId { get; set; }

    public int ExpenseVersionNumber { get; set; }

    public DateOnly DebtDate { get; set; }

    public DateOnly DueDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal DebtAmount { get; set; }

    public decimal AdjustmentAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal CreditNoteAmount { get; set; }

    public decimal WrittenOffAmount { get; set; }

    public decimal? OutstandingAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public Guid RecordedByUserId { get; set; }

    public DateTime? SettledAt { get; set; }

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

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Expense Expense { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<SupplierCreditNoteAllocation> SupplierCreditNoteAllocations { get; set; } = new List<SupplierCreditNoteAllocation>();

    public virtual SupplierDebtAdjustment? SupplierDebtAdjustment { get; set; }

    public virtual ICollection<SupplierDebtLedgerEntry> SupplierDebtLedgerEntries { get; set; } = new List<SupplierDebtLedgerEntry>();

    public virtual SupplierDebtWriteOff? SupplierDebtWriteOff { get; set; }

    public virtual ICollection<SupplierPaymentDebtAllocation> SupplierPaymentDebtAllocations { get; set; } = new List<SupplierPaymentDebtAllocation>();
}
