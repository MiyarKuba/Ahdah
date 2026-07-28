using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class OwnerRefundFundingSource
{
    public Guid OwnerRefundFundingSourceId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid OwnerPaymentRefundId { get; set; }

    public Guid FundingSourceId { get; set; }

    public decimal AllocatedAmount { get; set; }

    public Guid AllocatedByUserId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual FundingSource FundingSource { get; set; } = null!;

    public virtual OwnerPaymentRefund OwnerPaymentRefund { get; set; } = null!;
}
