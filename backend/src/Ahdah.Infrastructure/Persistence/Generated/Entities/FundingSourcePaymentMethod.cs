using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class FundingSourcePaymentMethod
{
    public Guid FundingSourcePaymentMethodId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid FundingSourceId { get; set; }

    public short SequenceNumber { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? PayerName { get; set; }

    public string? BankName { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? ProofFileUrl { get; set; }

    public string? Notes { get; set; }

    public Guid RecordedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual FundingSource FundingSource { get; set; } = null!;
}
