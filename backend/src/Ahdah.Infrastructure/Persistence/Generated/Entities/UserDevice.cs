using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class UserDevice
{
    public Guid DeviceId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserId { get; set; }

    public string DeviceIdentifier { get; set; } = null!;

    public string? DeviceName { get; set; }

    public string Platform { get; set; } = null!;

    public string? OperatingSystemVersion { get; set; }

    public string? AppVersion { get; set; }

    public string? PushToken { get; set; }

    public bool IsTrusted { get; set; }

    public bool IsActive { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? TrustedAt { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public DateTime? DeactivatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;
}
