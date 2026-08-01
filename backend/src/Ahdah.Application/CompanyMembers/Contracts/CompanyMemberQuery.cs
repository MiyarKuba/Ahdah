using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Access.Contracts;

namespace Ahdah.Application.CompanyMembers.Contracts;

public sealed record CompanyMemberQuery : PagedAccessQuery
{
    [RegularExpression("^(Manager|Deputy|Accountant|Supervisor|Worker)$")]
    public string? Role { get; init; }

    [RegularExpression("^(PendingApproval|Active|Suspended|Inactive|Rejected)$")]
    public string? Status { get; init; }

    [StringLength(100, MinimumLength = 2)]
    [RegularExpression("^[^\\r\\n]+$")]
    public string? Search { get; init; }
}
