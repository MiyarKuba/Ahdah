using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class ProjectContractChange
{
    public Guid ProjectContractChangeId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectId { get; set; }

    public string ChangeType { get; set; } = null!;

    public decimal PreviousContractValue { get; set; }

    public decimal ChangeAmount { get; set; }

    public decimal NewContractValue { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public string Reason { get; set; } = null!;

    public string? SupportingDocumentUrl { get; set; }

    public string Status { get; set; } = null!;

    public Guid RequestedByUserId { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? RejectionReason { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
