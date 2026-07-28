using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Advance
{
    public Guid AdvanceId { get; set; }

    public Guid CompanyId { get; set; }

    public string AdvanceNumber { get; set; } = null!;

    public Guid DeputyUserId { get; set; }

    public decimal AdvanceAmount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public DateOnly? SettlementDueDate { get; set; }

    public string Purpose { get; set; } = null!;

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? ConfirmedByUserId { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Guid? ReversedByUserId { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AdvanceClosure? AdvanceClosure { get; set; }

    public virtual ICollection<AdvanceFundingSource> AdvanceFundingSources { get; set; } = new List<AdvanceFundingSource>();

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser AppUser2 { get; set; } = null!;

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual MoneyTransfer? MoneyTransfer { get; set; }

    public virtual ICollection<TransferAdvanceAllocation> TransferAdvanceAllocations { get; set; } = new List<TransferAdvanceAllocation>();

    public virtual ICollection<UserAdvanceBalance> UserAdvanceBalances { get; set; } = new List<UserAdvanceBalance>();
}
