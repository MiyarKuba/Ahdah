using System.Text;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Abstractions.Context;
using Ahdah.Application.Access;
using Ahdah.Application.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Ahdah.Api.Authentication;

public static class AuthenticationDependencyInjection
{
    public static IServiceCollection AddAhdahAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = JwtOptions.FromConfiguration(configuration);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        services.AddSingleton(jwtOptions);
        services.AddSingleton<IAccessTokenService, JwtAccessTokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = AhdahClaimTypes.Name,
                    RoleClaimType = AhdahClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        await AuthenticationProblemWriter.WriteAsync(
                            context.HttpContext,
                            StatusCodes.Status401Unauthorized,
                            "Unauthorized",
                            "Authentication is required.",
                            "authentication.required");
                    },
                    OnForbidden = context => AuthenticationProblemWriter.WriteAsync(
                        context.HttpContext,
                        StatusCodes.Status403Forbidden,
                        "Forbidden",
                        "You are not authorized to perform this operation.",
                        "authorization.forbidden")
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AhdahAuthorizationPolicies.AuthenticatedUser,
                policy => policy.RequireAuthenticatedUser())
            .AddPolicy(AhdahAuthorizationPolicies.CompanyMember,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(AhdahClaimTypes.CompanyId)
                    .RequireAssertion(context => Guid.TryParse(
                        context.User.FindFirst(AhdahClaimTypes.CompanyId)?.Value,
                        out _)))
            .AddPolicy(AhdahAuthorizationPolicies.CompanyDirectoryViewer,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(AhdahClaimTypes.CompanyId)
                    .RequireAssertion(context => Guid.TryParse(
                        context.User.FindFirst(AhdahClaimTypes.CompanyId)?.Value,
                        out _))
                    .RequireRole(IdentityConstants.ManagerRole, AccessConstants.DeputyRole))
            .AddPolicy(AhdahAuthorizationPolicies.ProjectViewer,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(AhdahClaimTypes.CompanyId)
                    .RequireAssertion(context => Guid.TryParse(
                        context.User.FindFirst(AhdahClaimTypes.CompanyId)?.Value,
                        out _))
                    .RequireRole(
                        IdentityConstants.ManagerRole,
                        AccessConstants.DeputyRole,
                        AccessConstants.AccountantRole,
                        AccessConstants.SupervisorRole,
                        AccessConstants.WorkerRole))
            .AddPolicy(AhdahAuthorizationPolicies.ManagerOnly,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(AhdahClaimTypes.CompanyId)
                    .RequireAssertion(context => Guid.TryParse(
                        context.User.FindFirst(AhdahClaimTypes.CompanyId)?.Value,
                        out _))
                    .RequireRole(IdentityConstants.ManagerRole));

        return services;
    }
}

internal static class AuthenticationProblemWriter
{
    public static Task WriteAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string code)
    {
        return Results.Problem(
                statusCode: statusCode,
                title: title,
                detail: detail,
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = code,
                    ["traceId"] = httpContext.TraceIdentifier
                })
            .ExecuteAsync(httpContext);
    }
}
