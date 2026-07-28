using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class Project
{
    public Guid ProjectId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProjectOwnerId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string SiteAddress { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? ContactPhoneNumber { get; set; }

    public decimal ContractValue { get; set; }

    public DateOnly ContractDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? ExpectedEndDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid? CancelledByUserId { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<OwnerPaymentProjectAllocation> OwnerPaymentProjectAllocations { get; set; } = new List<OwnerPaymentProjectAllocation>();

    public virtual ICollection<OwnerPaymentRefund> OwnerPaymentRefunds { get; set; } = new List<OwnerPaymentRefund>();

    public virtual ICollection<PersonalClaim> PersonalClaims { get; set; } = new List<PersonalClaim>();

    public virtual ProjectContractChange? ProjectContractChange { get; set; }

    public virtual ProjectOwner ProjectOwner { get; set; } = null!;

    public virtual ICollection<ProjectSupervisor> ProjectSupervisors { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<SupplierDebt> SupplierDebts { get; set; } = new List<SupplierDebt>();
}
