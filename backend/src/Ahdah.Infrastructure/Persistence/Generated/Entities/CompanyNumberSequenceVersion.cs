using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CompanyNumberSequenceVersion
{
    public Guid CompanyNumberSequenceVersionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid CompanyNumberSequenceId { get; set; }

    public int VersionNumber { get; set; }

    public int? PreviousVersionNumber { get; set; }

    public int? RestoredFromVersionNumber { get; set; }

    public string ChangeType { get; set; } = null!;

    public string ChangeSummary { get; set; } = null!;

    public Guid? ChangeAllocationId { get; set; }

    public string SequenceCodeSnapshot { get; set; } = null!;

    public string SequenceNameSnapshot { get; set; } = null!;

    public string EntityTypeSnapshot { get; set; } = null!;

    public string? DescriptionSnapshot { get; set; }

    public string FormatTemplateSnapshot { get; set; } = null!;

    public string ResetPolicySnapshot { get; set; } = null!;

    public long StartValueSnapshot { get; set; }

    public long LastValueSnapshot { get; set; }

    public int IncrementBySnapshot { get; set; }

    public short PaddingLengthSnapshot { get; set; }

    public long? MaximumValueSnapshot { get; set; }

    public bool AllowCycleSnapshot { get; set; }

    public string AllocationModeSnapshot { get; set; } = null!;

    public string? CurrentPeriodKeySnapshot { get; set; }

    public DateTime? CurrentPeriodStartedAtSnapshot { get; set; }

    public DateTime? NextResetAtSnapshot { get; set; }

    public string? LastIssuedNumberSnapshot { get; set; }

    public DateTime? LastIssuedAtSnapshot { get; set; }

    public Guid? LastIssuedByUserIdSnapshot { get; set; }

    public Guid? LastAllocationIdSnapshot { get; set; }

    public string StatusSnapshot { get; set; } = null!;

    public string RecordedByType { get; set; } = null!;

    public Guid? RecordedByUserId { get; set; }

    public DateTime SourceSequenceUpdatedAt { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual CompanyNumberSequence CompanyNumberSequence { get; set; } = null!;

    public virtual CompanyNumberSequenceAllocation? CompanyNumberSequenceAllocation { get; set; }

    public virtual CompanyNumberSequenceAllocation? CompanyNumberSequenceAllocationNavigation { get; set; }

    public virtual CompanyNumberSequenceVersion? CompanyNumberSequenceVersion1 { get; set; }

    public virtual CompanyNumberSequenceVersion? CompanyNumberSequenceVersionNavigation { get; set; }

    public virtual ICollection<CompanyNumberSequenceVersion> InverseCompanyNumberSequenceVersion1 { get; set; } = new List<CompanyNumberSequenceVersion>();

    public virtual ICollection<CompanyNumberSequenceVersion> InverseCompanyNumberSequenceVersionNavigation { get; set; } = new List<CompanyNumberSequenceVersion>();
}
