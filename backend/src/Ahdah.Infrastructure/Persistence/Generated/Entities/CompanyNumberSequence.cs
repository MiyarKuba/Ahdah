using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CompanyNumberSequence
{
    public Guid CompanyNumberSequenceId { get; set; }

    public Guid CompanyId { get; set; }

    public string SequenceCode { get; set; } = null!;

    public string SequenceName { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public string? Description { get; set; }

    public string FormatTemplate { get; set; } = null!;

    public string ResetPolicy { get; set; } = null!;

    public long StartValue { get; set; }

    public long LastValue { get; set; }

    public int IncrementBy { get; set; }

    public short PaddingLength { get; set; }

    public long? MaximumValue { get; set; }

    public bool AllowCycle { get; set; }

    public string AllocationMode { get; set; } = null!;

    public string? CurrentPeriodKey { get; set; }

    public DateTime? CurrentPeriodStartedAt { get; set; }

    public DateTime? NextResetAt { get; set; }

    public string? LastIssuedNumber { get; set; }

    public DateTime? LastIssuedAt { get; set; }

    public Guid? LastIssuedByUserId { get; set; }

    public Guid? LastAllocationId { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public Guid? ActivatedByUserId { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public Guid? PausedByUserId { get; set; }

    public DateTime? PausedAt { get; set; }

    public string? PauseReason { get; set; }

    public Guid? RetiredByUserId { get; set; }

    public DateTime? RetiredAt { get; set; }

    public string? RetirementReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser? AppUser3 { get; set; }

    public virtual AppUser AppUser4 { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<CompanyNumberSequenceAllocation> CompanyNumberSequenceAllocations { get; set; } = new List<CompanyNumberSequenceAllocation>();

    public virtual ICollection<CompanyNumberSequenceVersion> CompanyNumberSequenceVersions { get; set; } = new List<CompanyNumberSequenceVersion>();
}
