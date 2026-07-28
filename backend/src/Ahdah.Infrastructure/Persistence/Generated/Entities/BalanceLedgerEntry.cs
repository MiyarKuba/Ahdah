using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class BalanceLedgerEntry
{
    public Guid BalanceLedgerEntryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserAdvanceBalanceId { get; set; }

    public int BalanceVersionNumber { get; set; }

    public string EntryType { get; set; } = null!;

    public decimal DeltaReceivedAmount { get; set; }

    public decimal DeltaRestoredAmount { get; set; }

    public decimal DeltaExpensedAmount { get; set; }

    public decimal DeltaTransferredOutAmount { get; set; }

    public decimal DeltaReturnedAmount { get; set; }

    public decimal DeltaAdjustmentOutAmount { get; set; }

    public decimal DeltaAvailableAmount { get; set; }

    public decimal DeltaReservedAmount { get; set; }

    public decimal ReceivedAfterAmount { get; set; }

    public decimal RestoredAfterAmount { get; set; }

    public decimal ExpensedAfterAmount { get; set; }

    public decimal TransferredOutAfterAmount { get; set; }

    public decimal ReturnedAfterAmount { get; set; }

    public decimal AdjustmentOutAfterAmount { get; set; }

    public decimal AvailableAfterAmount { get; set; }

    public decimal ReservedAfterAmount { get; set; }

    public string? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public Guid CorrelationId { get; set; }

    public string? Description { get; set; }

    public Guid PerformedByUserId { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual UserAdvanceBalance UserAdvanceBalance { get; set; } = null!;
}
