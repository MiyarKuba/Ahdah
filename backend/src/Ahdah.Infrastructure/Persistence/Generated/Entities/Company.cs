using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Company
{
    public Guid CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string CompanyCode { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public string Status { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Advance> Advances { get; set; } = new List<Advance>();

    public virtual AppUser? AppUser { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BackgroundJob> BackgroundJobs { get; set; } = new List<BackgroundJob>();

    public virtual ICollection<CompanyNumberSequence> CompanyNumberSequences { get; set; } = new List<CompanyNumberSequence>();

    public virtual CompanySetting? CompanySetting { get; set; }

    public virtual ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();

    public virtual ICollection<FundingSource> FundingSources { get; set; } = new List<FundingSource>();

    public virtual ICollection<IdempotencyRecord> IdempotencyRecords { get; set; } = new List<IdempotencyRecord>();

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    public virtual ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OutboxMessage> OutboxMessages { get; set; } = new List<OutboxMessage>();

    public virtual ICollection<ProjectOwner> ProjectOwners { get; set; } = new List<ProjectOwner>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<ReportDefinition> ReportDefinitions { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportRecipientSuppression> ReportRecipientSuppressions { get; set; } = new List<ReportRecipientSuppression>();

    public virtual ICollection<ReportSchedule> ReportSchedules { get; set; } = new List<ReportSchedule>();

    public virtual ICollection<SavedReportView> SavedReportViews { get; set; } = new List<SavedReportView>();

    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitions { get; set; } = new List<ScheduledJobDefinition>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
