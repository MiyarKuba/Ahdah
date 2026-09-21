using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class SupplierCreditIntent
{
    public Guid SupplierCreditIntentId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SupplierCreditNoteId { get; set; }

    public Guid? ProjectId { get; set; }

    public string IntentKind { get; set; } = null!;

    public decimal IntendedAmount { get; set; }

    public decimal ConsumedAmount { get; set; }

    public decimal ReleasedAmount { get; set; }

    public string Status { get; set; } = null!;

    public Guid PreparedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string Reason { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser AppUserNavigation { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual ICollection<SupplierCreditIntentApplication> SupplierCreditIntentApplications { get; set; } = new List<SupplierCreditIntentApplication>();

    public virtual SupplierCreditNote SupplierCreditNote { get; set; } = null!;
}
