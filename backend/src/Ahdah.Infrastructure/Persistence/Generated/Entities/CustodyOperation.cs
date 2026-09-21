using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CustodyOperation
{
    public Guid CustodyOperationId { get; set; }

    public Guid CompanyId { get; set; }

    public string OperationType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public Guid RequestedByUserId { get; set; }

    public DateTime RequestedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? AuthorizationDecisionId { get; set; }

    public string Reason { get; set; } = null!;

    public Guid IdempotencyRecordId { get; set; }

    public Guid? SourceOperationId { get; set; }

    public Guid? MoneyTransferId { get; set; }

    public Guid? ExpenseId { get; set; }

    public Guid? ExpenseReturnId { get; set; }

    public Guid? LegacyAttributionReviewId { get; set; }

    public Guid? AdvanceSettlementResolutionId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AdvanceSettlementResolution? AdvanceSettlementResolution { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<CustodyMovementEntry> CustodyMovementEntries { get; set; } = new List<CustodyMovementEntry>();

    public virtual CustodyOperation? CustodyOperationNavigation { get; set; }

    public virtual ICollection<CustodyReservation> CustodyReservationCustodyOperationNavigations { get; set; } = new List<CustodyReservation>();

    public virtual ICollection<CustodyReservation> CustodyReservationCustodyOperations { get; set; } = new List<CustodyReservation>();

    public virtual Expense? Expense { get; set; }

    public virtual ExpenseReturn? ExpenseReturn { get; set; }

    public virtual IdempotencyRecord IdempotencyRecord { get; set; } = null!;

    public virtual ICollection<CustodyOperation> InverseCustodyOperationNavigation { get; set; } = new List<CustodyOperation>();

    public virtual LegacyAttributionReview? LegacyAttributionReview { get; set; }

    public virtual MoneyTransfer? MoneyTransfer { get; set; }

    public virtual ProjectAuthorizationDecision? ProjectAuthorizationDecision { get; set; }
}
