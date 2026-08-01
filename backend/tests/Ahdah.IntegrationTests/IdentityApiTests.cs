using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Access.Services;
using Ahdah.Application.Identity;
using Ahdah.Application.Identity.Contracts;
using Ahdah.Application.Identity.Models;
using Ahdah.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ahdah.IntegrationTests;

public sealed class IdentityApiTests(IdentityApiFactory factory)
    : IClassFixture<IdentityApiFactory>
{
    [Fact]
    public async Task Health_endpoint_is_anonymous()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/api/system/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Me_returns_401_without_access_token()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Valid_access_token_satisfies_authenticated_company_member_endpoint()
    {
        using var client = factory.CreateSecureClient();
        var tokenService = factory.Services.GetRequiredService<IAccessTokenService>();
        var token = tokenService.CreateToken(new AccessTokenSubject(
            IdentityApiFactory.UserId,
            IdentityApiFactory.CompanyId,
            IdentityConstants.ManagerRole,
            "Test Manager"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_credentials_response_is_generic_and_non_disclosing()
    {
        using var client = factory.CreateSecureClient();
        const string attemptedPhone = "+218912345678";

        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(attemptedPhone, "incorrect-password"));
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("The supplied credentials are invalid.", body, StringComparison.Ordinal);
        Assert.DoesNotContain(attemptedPhone, body, StringComparison.Ordinal);
        Assert.DoesNotContain("company", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Token_without_company_id_does_not_satisfy_company_member_policy()
    {
        var authorization = factory.Services.GetRequiredService<IAuthorizationService>();
        var principal = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

        var result = await authorization.AuthorizeAsync(
            principal,
            resource: null,
            AhdahAuthorizationPolicies.CompanyMember);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Non_manager_role_does_not_satisfy_manager_only_policy()
    {
        var authorization = factory.Services.GetRequiredService<IAuthorizationService>();
        var principal = CreatePrincipal(
            new Claim(AhdahClaimTypes.CompanyId, Guid.NewGuid().ToString()),
            new Claim(AhdahClaimTypes.Role, "Worker"));

        var result = await authorization.AuthorizeAsync(
            principal,
            resource: null,
            AhdahAuthorizationPolicies.ManagerOnly);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task OpenApi_document_builds_without_persistence_entities()
    {
        using var client = factory.CreateSecureClient();

        var response = await client.GetAsync("/openapi/v1.json");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("passwordHash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tokenHash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("invitationCodeHash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Configured_flutter_web_origin_receives_controlled_cors_headers()
    {
        using var client = factory.CreateSecureClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/login");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "authorization,content-type");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(
            "http://localhost:5173",
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
        var allowedHeaders = Assert.Single(response.Headers.GetValues("Access-Control-Allow-Headers"));
        Assert.Contains("authorization", allowedHeaders, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("content-type", allowedHeaders, StringComparison.OrdinalIgnoreCase);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    [Fact]
    public async Task Unconfigured_origin_receives_no_cors_allow_origin_header()
    {
        using var client = factory.CreateSecureClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/login");
        request.Headers.Add("Origin", "https://untrusted.example");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await client.SendAsync(request);

        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims) => new(
        new ClaimsIdentity(claims, "Test", AhdahClaimTypes.Name, AhdahClaimTypes.Role));
}

public sealed class IdentityApiFactory : WebApplicationFactory<Program>
{
    internal static readonly Guid UserId = Guid.Parse("2e77a184-b59e-4e99-a276-37582a14b669");
    internal static readonly Guid CompanyId = Guid.Parse("f29b18bb-9954-46c5-ad07-bd48db68bb55");

    public HttpClient CreateSecureClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:AhdahDatabase"] =
                    "Host=localhost;Database=identity_tests_never_connected;Username=unused;Password=unused",
                ["Authentication:Jwt:Issuer"] = "Ahdah.Api.Tests",
                ["Authentication:Jwt:Audience"] = "Ahdah.Test.Client",
                ["Authentication:Jwt:AccessTokenMinutes"] = "15",
                ["Authentication:Jwt:SigningKey"] =
                    "integration-test-signing-key-material-at-least-32-bytes",
                ["Access:Invitations:LifetimeHours"] = "168"
            }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IIdentityService>();
            services.AddSingleton<IIdentityService, FakeIdentityService>();
            services.RemoveAll<IInvitationService>();
            services.AddSingleton<IInvitationService, FakeInvitationService>();
            services.RemoveAll<IJoinRequestService>();
            services.AddSingleton<IJoinRequestService, FakeJoinRequestService>();
        });
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public Task<IdentityResult<RegisterCompanyResult>> RegisterCompanyAsync(
            RegisterCompanyRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult<RegisterCompanyResult>.Failure(IdentityResultStatus.Conflict));

        public Task<IdentityResult<AuthenticationResult>> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult<AuthenticationResult>.Failure(
                IdentityResultStatus.InvalidCredentials));

        public Task<IdentityResult<CurrentUserResult>> GetCurrentUserAsync(
            CancellationToken cancellationToken)
        {
            var user = new UserSummary(UserId, "Test Manager", IdentityConstants.ManagerRole, "Active");
            var company = new CompanySummary(CompanyId, "Test Company", "TEST01", "Active");
            return Task.FromResult(IdentityResult<CurrentUserResult>.Success(
                new CurrentUserResult(user, company, IdentityConstants.ManagerRole)));
        }
    }
}
