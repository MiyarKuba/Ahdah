using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class NotificationTemplate
{
    public Guid NotificationTemplateId { get; set; }

    public Guid CompanyId { get; set; }

    public string TemplateName { get; set; } = null!;

    public string NotificationCode { get; set; } = null!;

    public string NotificationCategory { get; set; } = null!;

    public string DeliveryChannel { get; set; } = null!;

    public string LocaleCode { get; set; } = null!;

    public string DefaultPriority { get; set; } = null!;

    public string? TitleTemplate { get; set; }

    public string? SubjectTemplate { get; set; }

    public string BodyTemplate { get; set; } = null!;

    public string? HtmlBodyTemplate { get; set; }

    public string ActionType { get; set; } = null!;

    public string? ActionTargetTemplate { get; set; }

    public string PayloadTemplate { get; set; } = null!;

    public string RequiredVariables { get; set; } = null!;

    public string SampleData { get; set; } = null!;

    public string? Description { get; set; }

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

    public virtual ICollection<NotificationTemplateVersion> NotificationTemplateVersions { get; set; } = new List<NotificationTemplateVersion>();
}
