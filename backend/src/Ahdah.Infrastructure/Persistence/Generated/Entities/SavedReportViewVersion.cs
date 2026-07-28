using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SavedReportViewVersion
{
    public Guid SavedReportViewVersionId { get; set; }

    public long VersionSequence { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SavedReportViewId { get; set; }

    public Guid ReportDefinitionIdSnapshot { get; set; }

    public Guid OwnerUserIdSnapshot { get; set; }

    public Guid? ClonedFromSavedReportViewIdSnapshot { get; set; }

    public int ViewVersionNumber { get; set; }

    public int? PreviousViewVersionNumber { get; set; }

    public string ChangeType { get; set; } = null!;

    public string? PreviousStatus { get; set; }

    public string StatusSnapshot { get; set; } = null!;

    public string? ViewCodeSnapshot { get; set; }

    public string ViewNameSnapshot { get; set; } = null!;

    public string? DescriptionSnapshot { get; set; }

    public string ViewOriginSnapshot { get; set; } = null!;

    public string VisibilitySnapshot { get; set; } = null!;

    public string ReportVersionPolicySnapshot { get; set; } = null!;

    public int? PinnedReportVersionNumberSnapshot { get; set; }

    public string ParametersSnapshot { get; set; } = null!;

    public string FilterDefinitionsSnapshot { get; set; } = null!;

    public string SelectedColumnsSnapshot { get; set; } = null!;

    public string GroupingDefinitionsSnapshot { get; set; } = null!;

    public string SortingDefinitionsSnapshot { get; set; } = null!;

    public string AggregationDefinitionsSnapshot { get; set; } = null!;

    public string LayoutSettingsSnapshot { get; set; } = null!;

    public string ExportSettingsSnapshot { get; set; } = null!;

    public string PreferredExportFormatSnapshot { get; set; } = null!;

    public bool ContainsSensitiveParametersSnapshot { get; set; }

    public bool AllowSchedulingSnapshot { get; set; }

    public bool IsDefaultForOwnerSnapshot { get; set; }

    public bool IsCompanyDefaultSnapshot { get; set; }

    public bool IsFavoriteSnapshot { get; set; }

    public long RunCountSnapshot { get; set; }

    public Guid? LastReportRunIdSnapshot { get; set; }

    public DateTime? LastRunAtSnapshot { get; set; }

    public Guid CreatedByUserIdSnapshot { get; set; }

    public Guid UpdatedByUserIdSnapshot { get; set; }

    public Guid? ArchivedByUserIdSnapshot { get; set; }

    public DateTime? ArchivedAtSnapshot { get; set; }

    public string? ArchiveReasonSnapshot { get; set; }

    public Guid? DeletedByUserIdSnapshot { get; set; }

    public DateTime? DeletedAtSnapshot { get; set; }

    public string? DeletionReasonSnapshot { get; set; }

    public string ChangedByType { get; set; } = null!;

    public Guid? ChangedByUserId { get; set; }

    public string ChangeSummary { get; set; } = null!;

    public string ChangedFields { get; set; } = null!;

    public string? PreviousSnapshotHash { get; set; }

    public string SnapshotHash { get; set; } = null!;

    public Guid? RequestId { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUser3 { get; set; } = null!;

    public virtual AppUser AppUser4 { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual ReportDefinitionVersion? ReportDefinitionVersion { get; set; }

    public virtual ReportRun? ReportRun { get; set; }

    public virtual SavedReportView? SavedReportView { get; set; }

    public virtual SavedReportView SavedReportViewNavigation { get; set; } = null!;
}
