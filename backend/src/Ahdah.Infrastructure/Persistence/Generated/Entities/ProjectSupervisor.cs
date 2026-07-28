using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectSupervisor
{
    public Guid ProjectSupervisorId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid SupervisorUserId { get; set; }

    public Guid AssignedByUserId { get; set; }

    public DateTime AssignedAt { get; set; }

    public bool IsActive { get; set; }

    public Guid? RemovedByUserId { get; set; }

    public DateTime? RemovedAt { get; set; }

    public string? RemovalReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual AppUser AppUser1 { get; set; } = null!;

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Project Project { get; set; } = null!;
}
