using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class AdvanceClosure
{
    public Guid AdvanceClosureId { get; set; }

    public Guid CompanyId { get; set; }

    public string ClosureNumber { get; set; } = null!;

    public Guid AdvanceId { get; set; }

    public int AdvanceVersionNumber { get; set; }

    public DateOnly ClosureDate { get; set; }

    public string ClosureType { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public int BalanceCount { get; set; }

    public int SettledBalanceCount { get; set; }

    public int? UnsettledBalanceCount { get; set; }

    public int PendingOperationCount { get; set; }

    public int UnresolvedIssueCount { get; set; }

    public decimal TotalAvailableAmount { get; set; }

    public decimal TotalReservedAmount { get; set; }

    public decimal UnresolvedDifferenceAmount { get; set; }

    public decimal SettledShortageAmount { get; set; }

    public decimal SettledSurplusAmount { get; set; }

    public decimal OutstandingSupplierDebtAmount { get; set; }

    public decimal OutstandingPersonalClaimAmount { get; set; }

    public string? ReconciliationStatus { get; set; }

    public string ClosureReason { get; set; } = null!;

    public string? ReviewNotes { get; set; }

    public string? AuthorizationDocumentUrl { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid? SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? RejectedByUserId { get; set; }

    public DateTime? RejectedAt { get; set; }

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

    public virtual Advance Advance { get; set; } = null!;

    public virtual AdvanceClosureDocument? AdvanceClosureDocument { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser? AppUser4 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }
}
