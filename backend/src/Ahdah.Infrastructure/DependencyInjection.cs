using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Security;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Advances.Services;
using Ahdah.Application.CompanyMembers.Services;
using Ahdah.Application.Identity.Services;
using Ahdah.Application.Projects.Services;
using Ahdah.Infrastructure.Access.Invitations;
using Ahdah.Infrastructure.Access.JoinRequests;
using Ahdah.Infrastructure.Authentication;
using Ahdah.Infrastructure.Advances;
using Ahdah.Infrastructure.CompanyMembers;
using Ahdah.Infrastructure.Identity;
using Ahdah.Infrastructure.Projects;
using Ahdah.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
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

    public static IServiceCollection AddAhdahAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<InvitationOptions>()
            .Bind(configuration.GetSection(InvitationOptions.SectionName))
            .Validate(
                options => options.LifetimeHours is >= 1 and <= 720,
                "Invitation lifetime must be between 1 and 720 hours.")
            .ValidateOnStart();
        services.AddSingleton<IInvitationTokenService, InvitationTokenService>();
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IJoinRequestService, JoinRequestService>();

        return services;
    }

    public static IServiceCollection AddAhdahCompanyStructure(this IServiceCollection services)
    {
        services.AddScoped<ICompanyMemberService, CompanyMemberService>();
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }

    public static IServiceCollection AddAhdahAdvances(this IServiceCollection services)
    {
        services.AddScoped<IAdvanceService, AdvanceService>();
        return services;
    }
}
