using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CompanyNumberSequenceAllocation
{
    public Guid CompanyNumberSequenceAllocationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid CompanyNumberSequenceId { get; set; }

    public int SequenceVersionNumber { get; set; }

    public string SequenceCodeSnapshot { get; set; } = null!;

    public string ResetPolicySnapshot { get; set; } = null!;

    public string PeriodKey { get; set; } = null!;

    public long AllocatedValue { get; set; }

    public string FormattedNumber { get; set; } = null!;

    public string AllocationMode { get; set; } = null!;

    public string AllocationStatus { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public Guid? EntityId { get; set; }

    public int? EntityVersionNumber { get; set; }

    public Guid? ReservationToken { get; set; }

    public DateTime? ReservedAt { get; set; }

    public DateTime? ReservationExpiresAt { get; set; }

    public DateTime? IssuedAt { get; set; }

    public Guid? ReleasedByUserId { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public string? ReleaseReason { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public Guid? VoidedByUserId { get; set; }

    public DateTime? VoidedAt { get; set; }

    public string? VoidReason { get; set; }

    public string AllocatedByType { get; set; } = null!;

    public Guid? AllocatedByUserId { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual CompanyNumberSequence CompanyNumberSequence { get; set; } = null!;

    public virtual ICollection<CompanyNumberSequenceVersion> CompanyNumberSequenceVersionCompanyNumberSequenceAllocationNavigations { get; set; } = new List<CompanyNumberSequenceVersion>();

    public virtual ICollection<CompanyNumberSequenceVersion> CompanyNumberSequenceVersionCompanyNumberSequenceAllocations { get; set; } = new List<CompanyNumberSequenceVersion>();
}
