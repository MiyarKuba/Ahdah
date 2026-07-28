using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class AdvanceSettlement
{
    public Guid AdvanceSettlementId { get; set; }

    public Guid CompanyId { get; set; }

    public string SettlementNumber { get; set; } = null!;

    public Guid UserAdvanceBalanceId { get; set; }

    public int BalanceVersionNumber { get; set; }

    public DateOnly SettlementDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal ReceivedSnapshotAmount { get; set; }

    public decimal RestoredSnapshotAmount { get; set; }

    public decimal ExpensedSnapshotAmount { get; set; }

    public decimal TransferredOutSnapshotAmount { get; set; }

    public decimal ReturnedSnapshotAmount { get; set; }

    public decimal AdjustmentOutSnapshotAmount { get; set; }

    public decimal AvailableSnapshotAmount { get; set; }

    public decimal ReservedSnapshotAmount { get; set; }

    public decimal DeclaredRemainingAmount { get; set; }

    public decimal? DifferenceAmount { get; set; }

    public string? DifferenceType { get; set; }

    public string? SettlementExplanation { get; set; }

    public string? SupportingDocumentUrl { get; set; }

    public string? ResolutionNotes { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewNotes { get; set; }

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

    public virtual AdvanceSettlementDocument? AdvanceSettlementDocument { get; set; }

    public virtual ICollection<AdvanceSettlementResolution> AdvanceSettlementResolutions { get; set; } = new List<AdvanceSettlementResolution>();

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual UserAdvanceBalance UserAdvanceBalance { get; set; } = null!;
}
