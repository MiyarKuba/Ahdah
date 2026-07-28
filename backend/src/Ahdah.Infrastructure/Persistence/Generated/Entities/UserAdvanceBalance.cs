using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class UserAdvanceBalance
{
    public Guid UserAdvanceBalanceId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid AdvanceId { get; set; }

    public Guid UserId { get; set; }

    public decimal TotalReceivedAmount { get; set; }

    public decimal TotalRestoredAmount { get; set; }

    public decimal TotalExpensedAmount { get; set; }

    public decimal TotalTransferredOutAmount { get; set; }

    public decimal TotalReturnedAmount { get; set; }

    public decimal TotalAdjustmentOutAmount { get; set; }

    public decimal AvailableAmount { get; set; }

    public decimal ReservedAmount { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public DateTime? SettledAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? Notes { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Advance Advance { get; set; } = null!;

    public virtual AdvanceSettlement? AdvanceSettlement { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual ICollection<BalanceLedgerEntry> BalanceLedgerEntries { get; set; } = new List<BalanceLedgerEntry>();

    public virtual ICollection<ExpenseAdvanceAllocation> ExpenseAdvanceAllocations { get; set; } = new List<ExpenseAdvanceAllocation>();
}
