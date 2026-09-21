using System;
using System.Collections.Generic;

namespace Ahdah.Infrastructure.Persistence.Generated.Entities;

public partial class LegacyAttributionReview
{
    public Guid LegacyAttributionReviewId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid BoundaryKey { get; set; }

    public string SourceType { get; set; } = null!;

    public Guid SourceId { get; set; }

    public int? SourceVersion { get; set; }

    public string Status { get; set; } = null!;

    public string UncertaintyScope { get; set; } = null!;

    public Guid? DeclaredByUserId { get; set; }

    public DateTime? DeclaredAt { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? EvidenceId { get; set; }

    public string DeclaredDetails { get; set; } = null!;

    public byte[] SourceDigest { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public Guid? SupersedesReviewId { get; set; }

    public int VersionNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual AppUser? AppUser1 { get; set; }

    public virtual AppUser? AppUserNavigation { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual CustodyOperation? CustodyOperation { get; set; }

    public virtual ICollection<LegacyAttributionReview> InverseLegacyAttributionReviewNavigation { get; set; } = new List<LegacyAttributionReview>();

    public virtual LegacyAttributionReview? LegacyAttributionReviewNavigation { get; set; }

    public virtual ICollection<LegacyAttributionReviewProject> LegacyAttributionReviewProjects { get; set; } = new List<LegacyAttributionReviewProject>();

    public virtual VerifiedEvidenceObject? VerifiedEvidenceObject { get; set; }
}
