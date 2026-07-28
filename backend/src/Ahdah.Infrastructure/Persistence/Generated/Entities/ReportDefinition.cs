using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ReportDefinition
{
    public Guid ReportDefinitionId { get; set; }

    public Guid CompanyId { get; set; }

    public string ReportCode { get; set; } = null!;

    public string ReportName { get; set; } = null!;

    public string ReportCategory { get; set; } = null!;

    public string DefinitionOrigin { get; set; } = null!;

    public string ReportScope { get; set; } = null!;

    public string DataSourceCode { get; set; } = null!;

    public string? Description { get; set; }

    public string? DefaultCurrencyCode { get; set; }

    public string DefaultLocaleCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public string ParameterSchema { get; set; } = null!;

    public string DefaultParameters { get; set; } = null!;

    public string ColumnDefinitions { get; set; } = null!;

    public string FilterDefinitions { get; set; } = null!;

    public string GroupingDefinitions { get; set; } = null!;

    public string SortingDefinitions { get; set; } = null!;

    public string AggregationDefinitions { get; set; } = null!;

    public string LayoutSettings { get; set; } = null!;

    public string AllowedRoles { get; set; } = null!;

    public string AllowedExportFormats { get; set; } = null!;

    public string DefaultExportFormat { get; set; } = null!;

    public int MaximumRowCount { get; set; }

    public int ExecutionTimeoutSeconds { get; set; }

    public int CacheTtlSeconds { get; set; }

    public int GeneratedFileRetentionDays { get; set; }

    public bool ContainsSensitiveData { get; set; }

    public bool RequiresExplicitExportPermission { get; set; }

    public bool AllowScheduling { get; set; }

    public bool AllowUserFilters { get; set; }

    public bool AllowUserColumnSelection { get; set; }

    public string Status { get; set; } = null!;

    public int CurrentVersionNumber { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public Guid? ActivatedByUserId { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public Guid? DisabledByUserId { get; set; }

    public DateTime? DisabledAt { get; set; }

    public string? DisabledReason { get; set; }

    public Guid? RetiredByUserId { get; set; }

    public DateTime? RetiredAt { get; set; }

    public string? RetirementReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUser2 { get; set; }

    public virtual AppUser AppUser3 { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<ReportDefinitionVersion> ReportDefinitionVersions { get; set; } = new List<ReportDefinitionVersion>();

    public virtual ICollection<ReportRecipientSuppressionEvent> ReportRecipientSuppressionEvents { get; set; } = new List<ReportRecipientSuppressionEvent>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressions { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ICollection<ReportSchedule> ReportSchedules { get; set; } = new List<ReportSchedule>();

    public virtual SavedReportView? SavedReportView { get; set; }
}
