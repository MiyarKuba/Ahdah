using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class LegacyAttributionReviewProject
{
    public Guid LegacyAttributionReviewProjectId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid LegacyAttributionReviewId { get; set; }

    public Guid ProjectId { get; set; }

    public string ImpactCode { get; set; } = null!;

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual LegacyAttributionReview LegacyAttributionReview { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
