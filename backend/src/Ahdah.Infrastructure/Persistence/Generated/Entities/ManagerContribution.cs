using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ManagerContribution
{
    public Guid ManagerContributionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid FundingSourceId { get; set; }

    public Guid ManagerUserId { get; set; }

    public DateOnly ContributionDate { get; set; }

    public decimal ContributionAmount { get; set; }

    public string Purpose { get; set; } = null!;

    public string Status { get; set; } = null!;

    public Guid RecordedByUserId { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? Notes { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public Guid? ReversedByUserId { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual FundingSource FundingSource { get; set; } = null!;

    public virtual PersonalClaim? PersonalClaim { get; set; }
}
