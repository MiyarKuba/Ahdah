using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CustodyMovementEntry
{
    public Guid CustodyMovementEntryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid CustodyOperationId { get; set; }

    public Guid ProjectCustodyPositionId { get; set; }

    public int EntryNumber { get; set; }

    public decimal DeltaAvailableAmount { get; set; }

    public decimal DeltaReservedAmount { get; set; }

    public decimal ReceivedAmount { get; set; }

    public decimal RestoredAmount { get; set; }

    public decimal ExpensedAmount { get; set; }

    public decimal TransferredOutAmount { get; set; }

    public decimal ReturnedAmount { get; set; }

    public decimal AdjustmentOutAmount { get; set; }

    public Guid? BalanceLedgerEntryId { get; set; }

    public Guid? ReversesEntryId { get; set; }

    public DateTime OccurredAt { get; set; }

    public Guid ActorUserId { get; set; }

    public int PositionVersionBefore { get; set; }

    public int PositionVersionAfter { get; set; }

    public long? FinancialRevisionAfter { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual BalanceLedgerEntry? BalanceLedgerEntry { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual CustodyMovementEntry? CustodyMovementEntryNavigation { get; set; }

    public virtual CustodyOperation CustodyOperation { get; set; } = null!;

    public virtual FinancialResolutionEffect? FinancialResolutionEffect { get; set; }

    public virtual ICollection<CustodyMovementEntry> InverseCustodyMovementEntryNavigation { get; set; } = new List<CustodyMovementEntry>();

    public virtual ProjectCustodyPosition ProjectCustodyPosition { get; set; } = null!;
}
