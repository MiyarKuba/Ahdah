using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportDefinitionVersion
{
    public Guid ReportDefinitionVersionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ReportDefinitionId { get; set; }

    public int VersionNumber { get; set; }

    public int? PreviousVersionNumber { get; set; }

    public int? RestoredFromVersionNumber { get; set; }

    public string ChangeType { get; set; } = null!;

    public string ChangeSummary { get; set; } = null!;

    public string ReportCodeSnapshot { get; set; } = null!;

    public string ReportNameSnapshot { get; set; } = null!;

    public string ReportCategorySnapshot { get; set; } = null!;

    public string DefinitionOriginSnapshot { get; set; } = null!;

    public string ReportScopeSnapshot { get; set; } = null!;

    public string DataSourceCodeSnapshot { get; set; } = null!;

    public string? DescriptionSnapshot { get; set; }

    public string? DefaultCurrencyCodeSnapshot { get; set; }

    public string DefaultLocaleCodeSnapshot { get; set; } = null!;

    public string TimeZoneSnapshot { get; set; } = null!;

    public string ParameterSchemaSnapshot { get; set; } = null!;

    public string DefaultParametersSnapshot { get; set; } = null!;

    public string ColumnDefinitionsSnapshot { get; set; } = null!;

    public string FilterDefinitionsSnapshot { get; set; } = null!;

    public string GroupingDefinitionsSnapshot { get; set; } = null!;

    public string SortingDefinitionsSnapshot { get; set; } = null!;

    public string AggregationDefinitionsSnapshot { get; set; } = null!;

    public string LayoutSettingsSnapshot { get; set; } = null!;

    public string AllowedRolesSnapshot { get; set; } = null!;

    public string AllowedExportFormatsSnapshot { get; set; } = null!;

    public string DefaultExportFormatSnapshot { get; set; } = null!;

    public int MaximumRowCountSnapshot { get; set; }

    public int ExecutionTimeoutSecondsSnapshot { get; set; }

    public int CacheTtlSecondsSnapshot { get; set; }

    public int GeneratedFileRetentionDaysSnapshot { get; set; }

    public bool ContainsSensitiveDataSnapshot { get; set; }

    public bool RequiresExportPermissionSnapshot { get; set; }

    public bool AllowSchedulingSnapshot { get; set; }

    public bool AllowUserFiltersSnapshot { get; set; }

    public bool AllowUserColumnSelectionSnapshot { get; set; }

    public string ReportStatusSnapshot { get; set; } = null!;

    public Guid RecordedByUserId { get; set; }

    public DateTime SourceReportUpdatedAt { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual ICollection<ReportDefinitionVersion> InverseReportDefinitionVersion1 { get; set; } = new List<ReportDefinitionVersion>();

    public virtual ICollection<ReportDefinitionVersion> InverseReportDefinitionVersionNavigation { get; set; } = new List<ReportDefinitionVersion>();

    public virtual ReportDefinition ReportDefinition { get; set; } = null!;

    public virtual ReportDefinitionVersion? ReportDefinitionVersion1 { get; set; }

    public virtual ReportDefinitionVersion? ReportDefinitionVersionNavigation { get; set; }

    public virtual ICollection<ReportRun> ReportRuns { get; set; } = new List<ReportRun>();

    public virtual ICollection<ReportSchedule> ReportSchedules { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<SavedReportViewVersion> SavedReportViewVersions { get; set; } = new List<SavedReportViewVersion>();

    public virtual ICollection<SavedReportView> SavedReportViews { get; set; } = new List<SavedReportView>();
}
