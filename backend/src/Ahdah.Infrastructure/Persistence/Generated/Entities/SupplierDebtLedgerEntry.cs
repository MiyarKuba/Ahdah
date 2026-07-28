using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierDebtLedgerEntry
{
    public Guid SupplierDebtLedgerEntryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SupplierDebtId { get; set; }

    public int DebtVersionNumber { get; set; }

    public string EntryType { get; set; } = null!;

    public decimal DeltaDebtAmount { get; set; }

    public decimal DeltaAdjustmentAmount { get; set; }

    public decimal DeltaPaidAmount { get; set; }

    public decimal DeltaCreditNoteAmount { get; set; }

    public decimal DeltaWrittenOffAmount { get; set; }

    public decimal DebtAfterAmount { get; set; }

    public decimal AdjustmentAfterAmount { get; set; }

    public decimal PaidAfterAmount { get; set; }

    public decimal CreditNoteAfterAmount { get; set; }

    public decimal WrittenOffAfterAmount { get; set; }

    public decimal OutstandingAfterAmount { get; set; }

    public string DebtStatusAfter { get; set; } = null!;

    public string? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public Guid CorrelationId { get; set; }

    public string? Description { get; set; }

    public Guid PerformedByUserId { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual SupplierDebt SupplierDebt { get; set; } = null!;
}
