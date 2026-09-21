using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class FinancialResolutionEffect
{
    public Guid FinancialResolutionEffectId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? ExpenseReturnId { get; set; }

    public Guid? AdvanceSettlementResolutionId { get; set; }

    public int EffectNumber { get; set; }

    public string EffectType { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Direction { get; set; } = null!;

    public Guid? CustodyMovementEntryId { get; set; }

    public Guid? FundingSourceLedgerEntryId { get; set; }

    public Guid? PersonalClaimAdjustmentId { get; set; }

    public Guid? PersonalClaimWriteOffId { get; set; }

    public Guid? SupplierDebtWriteOffId { get; set; }

    public Guid? SupplierCreditNoteId { get; set; }

    public Guid? SupplierRefundId { get; set; }

    public Guid EconomicEffectKey { get; set; }

    public string Polarity { get; set; } = null!;

    public Guid AuthorizationDecisionId { get; set; }

    public Guid? EvidenceId { get; set; }

    public Guid? ReversesEffectId { get; set; }

    public DateTime PostedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AdvanceSettlementResolution? AdvanceSettlementResolution { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual CustodyMovementEntry? CustodyMovementEntry { get; set; }

    public virtual ExpenseReturn? ExpenseReturn { get; set; }

    public virtual FinancialResolutionEffect? FinancialResolutionEffectNavigation { get; set; }

    public virtual FundingSourceLedgerEntry? FundingSourceLedgerEntry { get; set; }

    public virtual ICollection<FinancialResolutionEffect> InverseFinancialResolutionEffectNavigation { get; set; } = new List<FinancialResolutionEffect>();

    public virtual PersonalClaimAdjustment? PersonalClaimAdjustment { get; set; }

    public virtual PersonalClaimWriteOff? PersonalClaimWriteOff { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAuthorizationDecision ProjectAuthorizationDecision { get; set; } = null!;

    public virtual SupplierCreditNote? SupplierCreditNote { get; set; }

    public virtual SupplierDebtWriteOff? SupplierDebtWriteOff { get; set; }

    public virtual SupplierRefund? SupplierRefund { get; set; }

    public virtual VerifiedEvidenceObject? VerifiedEvidenceObject { get; set; }
}
