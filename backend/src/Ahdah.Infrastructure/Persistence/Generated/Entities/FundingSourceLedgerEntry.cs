using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class FundingSourceLedgerEntry
{
    public Guid FundingSourceLedgerEntryId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid FundingSourceId { get; set; }

    public int SourceVersionNumber { get; set; }

    public string EntryType { get; set; } = null!;

    public decimal Amount { get; set; }

    public decimal AvailableDelta { get; set; }

    public decimal ReservedDelta { get; set; }

    public decimal UsedDelta { get; set; }

    public decimal ReversedDelta { get; set; }

    public decimal AvailableAfter { get; set; }

    public decimal ReservedAfter { get; set; }

    public decimal UsedAfter { get; set; }

    public decimal ReversedAfter { get; set; }

    public string? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public Guid? PerformedByUserId { get; set; }

    public Guid CorrelationId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual FundingSource FundingSource { get; set; } = null!;
}
