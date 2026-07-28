using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class NotificationPreference
{
    public Guid NotificationPreferenceId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserId { get; set; }

    public string PreferenceScope { get; set; } = null!;

    public string? NotificationCategory { get; set; }

    public string? NotificationCode { get; set; }

    public string DeliveryChannel { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsMandatory { get; set; }

    public string MinimumPriority { get; set; } = null!;

    public string DeliveryMode { get; set; } = null!;

    public string? DigestFrequency { get; set; }

    public TimeOnly? DigestTime { get; set; }

    public short? DigestDayOfWeek { get; set; }

    public bool QuietHoursEnabled { get; set; }

    public TimeOnly? QuietHoursStart { get; set; }

    public TimeOnly? QuietHoursEnd { get; set; }

    public string TimeZone { get; set; } = null!;

    public DateTime? MutedUntil { get; set; }

    public bool AllowFallback { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser AppUserNavigation { get; set; } = null!;
}
