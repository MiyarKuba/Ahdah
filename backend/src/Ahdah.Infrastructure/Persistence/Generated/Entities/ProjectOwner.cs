using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectOwner
{
    public Guid ProjectOwnerId { get; set; }

    public Guid CompanyId { get; set; }

    public string OwnerName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public Guid CreatedByUserId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefunds { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<ProjectOwnerPayment> ProjectOwnerPayments { get; set; } = new List<ProjectOwnerPayment>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
