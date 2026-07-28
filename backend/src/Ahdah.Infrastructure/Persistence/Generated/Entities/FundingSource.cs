using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class FundingSource
{
    public Guid FundingSourceId { get; set; }

    public Guid CompanyId { get; set; }

    public string SourceType { get; set; } = null!;

    public DateOnly SourceDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal GrossAmount { get; set; }

    public decimal FeeAmount { get; set; }

    public decimal NetAmount { get; set; }

    public decimal AvailableAmount { get; set; }

    public decimal ReservedAmount { get; set; }

    public decimal UsedAmount { get; set; }

    public decimal ReversedAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Guid? ReversedByUserId { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AdvanceFundingSource> AdvanceFundingSources { get; set; } = new List<AdvanceFundingSource>();

    public virtual AdvanceSettlementResolution? AdvanceSettlementResolution { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual CompanyCashboxEntry? CompanyCashboxEntry { get; set; }

    public virtual ICollection<FundingSourceLedgerEntry> FundingSourceLedgerEntries { get; set; } = new List<FundingSourceLedgerEntry>();

    public virtual ICollection<FundingSourcePaymentMethod> FundingSourcePaymentMethods { get; set; } = new List<FundingSourcePaymentMethod>();

    public virtual ManagerContribution? ManagerContribution { get; set; }

    public virtual ICollection<OwnerRefundFundingSource> OwnerRefundFundingSources { get; set; } = new List<OwnerRefundFundingSource>();

    public virtual ICollection<PersonalClaimPaymentFundingSource> PersonalClaimPaymentFundingSources { get; set; } = new List<PersonalClaimPaymentFundingSource>();

    public virtual ProjectOwnerPayment? ProjectOwnerPayment { get; set; }

    public virtual ICollection<SupplierPaymentFundingSource> SupplierPaymentFundingSources { get; set; } = new List<SupplierPaymentFundingSource>();

    public virtual SupplierRefund? SupplierRefund { get; set; }
}
