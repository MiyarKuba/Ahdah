using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class NotificationTemplateVersion
{
    public Guid NotificationTemplateVersionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid NotificationTemplateId { get; set; }

    public int VersionNumber { get; set; }

    public int? PreviousVersionNumber { get; set; }

    public int? RestoredFromVersionNumber { get; set; }

    public string ChangeType { get; set; } = null!;

    public string ChangeSummary { get; set; } = null!;

    public string TemplateNameSnapshot { get; set; } = null!;

    public string NotificationCodeSnapshot { get; set; } = null!;

    public string NotificationCategorySnapshot { get; set; } = null!;

    public string DeliveryChannelSnapshot { get; set; } = null!;

    public string LocaleCodeSnapshot { get; set; } = null!;

    public string DefaultPrioritySnapshot { get; set; } = null!;

    public string? TitleTemplateSnapshot { get; set; }

    public string? SubjectTemplateSnapshot { get; set; }

    public string BodyTemplateSnapshot { get; set; } = null!;

    public string? HtmlBodyTemplateSnapshot { get; set; }

    public string ActionTypeSnapshot { get; set; } = null!;

    public string? ActionTargetTemplateSnapshot { get; set; }

    public string PayloadTemplateSnapshot { get; set; } = null!;

    public string RequiredVariablesSnapshot { get; set; } = null!;

    public string SampleDataSnapshot { get; set; } = null!;

    public string? DescriptionSnapshot { get; set; }

    public string TemplateStatusSnapshot { get; set; } = null!;

    public Guid RecordedByUserId { get; set; }

    public DateTime SourceTemplateUpdatedAt { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual ICollection<NotificationTemplateVersion> InverseNotificationTemplateVersion1 { get; set; } = new List<NotificationTemplateVersion>();

    public virtual ICollection<NotificationTemplateVersion> InverseNotificationTemplateVersionNavigation { get; set; } = new List<NotificationTemplateVersion>();

    public virtual NotificationTemplate NotificationTemplate { get; set; } = null!;

    public virtual NotificationTemplateVersion? NotificationTemplateVersion1 { get; set; }

    public virtual NotificationTemplateVersion? NotificationTemplateVersionNavigation { get; set; }
}
