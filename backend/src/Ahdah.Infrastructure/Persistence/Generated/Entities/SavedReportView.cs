using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SavedReportView
{
    public Guid SavedReportViewId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportDefinitionId { get; set; }

    public Guid OwnerUserId { get; set; }

    public Guid? ClonedFromSavedReportViewId { get; set; }

    public string? ViewCode { get; set; }

    public string ViewName { get; set; } = null!;

    public string? Description { get; set; }

    public string ViewOrigin { get; set; } = null!;

    public string Visibility { get; set; } = null!;

    public string ReportVersionPolicy { get; set; } = null!;

    public int? PinnedReportVersionNumber { get; set; }

    public string Parameters { get; set; } = null!;

    public string FilterDefinitions { get; set; } = null!;

    public string SelectedColumns { get; set; } = null!;

    public string GroupingDefinitions { get; set; } = null!;

    public string SortingDefinitions { get; set; } = null!;

    public string AggregationDefinitions { get; set; } = null!;

    public string LayoutSettings { get; set; } = null!;

    public string ExportSettings { get; set; } = null!;

    public string PreferredExportFormat { get; set; } = null!;

    public bool ContainsSensitiveParameters { get; set; }

    public bool AllowScheduling { get; set; }

    public bool IsDefaultForOwner { get; set; }

    public bool IsCompanyDefault { get; set; }

    public bool IsFavorite { get; set; }

    public long RunCount { get; set; }

    public Guid? LastReportRunId { get; set; }

    public DateTime? LastRunAt { get; set; }

    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public Guid? ArchivedByUserId { get; set; }

    public DateTime? ArchivedAt { get; set; }

    public string? ArchiveReason { get; set; }

    public Guid? DeletedByUserId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletionReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser AppUser2 { get; set; } = null!;

    public virtual AppUser AppUser3 { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<SavedReportView> InverseSavedReportViewNavigation { get; set; } = new List<SavedReportView>();

    public virtual ReportDefinition ReportDefinition { get; set; } = null!;

    public virtual ReportDefinitionVersion? ReportDefinitionVersion { get; set; }

    public virtual ReportRun? ReportRun { get; set; }

    public virtual SavedReportView? SavedReportViewNavigation { get; set; }

    public virtual ICollection<SavedReportViewShareEvent> SavedReportViewShareEvents { get; set; } = new List<SavedReportViewShareEvent>();

    public virtual ICollection<SavedReportViewShare> SavedReportViewShares { get; set; } = new List<SavedReportViewShare>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionSavedReportViewNavigations { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersionSavedReportViews { get; set; } = new List<SavedReportViewVersion>();
}
