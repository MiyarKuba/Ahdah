using Ahdah.Application.Identity.Contracts;
using Ahdah.Application.Identity.Models;

namespace Ahdah.Application.Identity.Services;

public interface IIdentityService
{
    Task<IdentityResult<RegisterCompanyResult>> RegisterCompanyAsync(
        RegisterCompanyRequest request,
        CancellationToken cancellationToken);

    Task<IdentityResult<AuthenticationResult>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<IdentityResult<CurrentUserResult>> GetCurrentUserAsync(
        CancellationToken cancellationToken);
}
