using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class CompanySettlementPolicyBinding
{
    public Guid CompanyId { get; set; }

    public Guid ActivePolicyVersionId { get; set; }

    public Guid ActivatedByUserId { get; set; }

    public DateTime ActivatedAt { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual SettlementPolicyVersion SettlementPolicyVersion { get; set; } = null!;
}
