using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class JoinRequest
{
    public Guid JoinRequestId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserId { get; set; }

    public string RequestedRole { get; set; } = null!;

    public string? AssignedRole { get; set; }

    public string Status { get; set; } = null!;

    public string? RequestMessage { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public string? ReviewNotes { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}
