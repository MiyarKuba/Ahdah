using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class PersonalClaim
{
    public Guid PersonalClaimId { get; set; }

    public Guid CompanyId { get; set; }

    public string ClaimNumber { get; set; } = null!;

    public Guid ClaimantUserId { get; set; }

    public Guid? ProjectId { get; set; }

    public string SourceType { get; set; } = null!;

    public Guid? ExpenseId { get; set; }

    public Guid? ManagerContributionId { get; set; }

    public DateOnly ClaimDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal ClaimAmount { get; set; }

    public decimal AdjustmentAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal ReductionAmount { get; set; }

    public decimal WrittenOffAmount { get; set; }

    public decimal? OutstandingAmount { get; set; }

    public string Description { get; set; } = null!;

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

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

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutions { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Expense? Expense { get; set; }

    public virtual ManagerContribution? ManagerContribution { get; set; }

    public virtual PersonalClaimAdjustment? PersonalClaimAdjustment { get; set; }

    public virtual ICollection<PersonalClaimLedgerEntry> PersonalClaimLedgerEntries { get; set; } = new List<PersonalClaimLedgerEntry>();

    public virtual ICollection<PersonalClaimPaymentAllocation> PersonalClaimPaymentAllocations { get; set; } = new List<PersonalClaimPaymentAllocation>();

    public virtual PersonalClaimWriteOff? PersonalClaimWriteOff { get; set; }

    public virtual Project? Project { get; set; }
}
