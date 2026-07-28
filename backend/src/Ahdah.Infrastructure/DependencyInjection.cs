using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Identity.Services;
using Ahdah.Infrastructure.Authentication;
using Ahdah.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAhdahIdentity(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
