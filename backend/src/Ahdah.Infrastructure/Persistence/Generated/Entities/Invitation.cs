using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Invitation
{
    public Guid InvitationId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid InvitedByUserId { get; set; }

    public Guid? AcceptedByUserId { get; set; }

    public string InvitedPhoneNumber { get; set; } = null!;

    public string AssignedRole { get; set; } = null!;

    public string InvitationCodeHash { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}
