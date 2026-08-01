using Ahdah.Application.Access.Models;
using Ahdah.Application.CompanyMembers.Contracts;
using Ahdah.Application.CompanyMembers.Models;

namespace Ahdah.Application.CompanyMembers.Services;

public interface ICompanyMemberService
{
    Task<AccessResult<PagedResult<CompanyMemberSummary>>> ListAsync(
        CompanyMemberQuery query,
        CancellationToken cancellationToken);

    Task<AccessResult<CompanyMemberDetails>> GetAsync(
        Guid memberId,
        CancellationToken cancellationToken);
}
