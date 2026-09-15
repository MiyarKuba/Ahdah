using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Application.Settlements;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ahdah.IntegrationTests;

public sealed class SettlementApiTests(IdentityApiFactory factory) : IClassFixture<IdentityApiFactory>
{
    [Theory]
    [InlineData("Manager", HttpStatusCode.OK)]
    [InlineData("Deputy", HttpStatusCode.OK)]
    [InlineData("Accountant", HttpStatusCode.OK)]
    [InlineData("Supervisor", HttpStatusCode.OK)]
    [InlineData("Worker", HttpStatusCode.Forbidden)]
    [InlineData("FutureRole", HttpStatusCode.Forbidden)]
    public async Task Endpoint_policy_and_real_service_enforce_the_role_matrix(string role, HttpStatusCode expected)
    {
        using var store = new SettlementStore(role);
        store.Assign();
        await store.Save();
        using var app = WithStore(store);
        using var client = Client(app, store);
        Assert.Equal(expected, (await client.GetAsync(Path(store.ProjectId))).StatusCode);
    }

    [Fact]
    public async Task Endpoint_requires_authentication()
    {
        using var client = factory.CreateSecureClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync(Path(Guid.NewGuid()))).StatusCode);
    }

    [Fact]
    public async Task Supervisor_unassigned_project_is_a_safe_404()
    {
        using var store = new SettlementStore("Supervisor");
        await store.Save();
        using var app = WithStore(store);
        using var client = Client(app, store);
        var response = await client.GetAsync(Path(store.ProjectId));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("projects.not_found", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Cross_tenant_project_and_missing_project_are_safe_404s()
    {
        using var store = new SettlementStore();
        store.Project.CompanyId = Guid.NewGuid();
        await store.Save();
        using var app = WithStore(store);
        using var client = Client(app, store);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(Path(store.ProjectId))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(Path(Guid.NewGuid()))).StatusCode);
    }

    [Fact]
    public async Task Read_contract_has_safe_references_exact_decimal_and_explicit_null_readiness()
    {
        using var store = new SettlementStore();
        store.Project.Status = "Completed";
        store.Project.CompletedAt = DateTime.UtcNow;
        store.Project.ActualEndDate = DateOnly.FromDateTime(DateTime.UtcNow);
        await store.Save();
        using var app = WithStore(store);
        using var client = Client(app, store);
        var clean = await client.GetStringAsync(Path(store.ProjectId));
        using var json = JsonDocument.Parse(clean);
        Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty("canSettle").ValueKind);
        Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty("canClose").ValueKind);
        Assert.Equal("Indeterminate", json.RootElement.GetProperty("closureReadiness").GetString());

        var debt = store.Debt("PartiallySettled", 90071992547409.91m);
        await store.Save();
        var body = await client.GetStringAsync(Path(store.ProjectId));
        Assert.Contains("90071992547409.91", body);
        Assert.Contains(debt.SupplierDebtId.ToString(), body);
        Assert.DoesNotContain("companyId", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("fileUrl", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("contractValue", body, StringComparison.OrdinalIgnoreCase);
        var result = JsonSerializer.Deserialize<ProjectSettlementSummary>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.False(result.CanSettle);
        Assert.False(result.CanClose);
    }

    [Fact]
    public async Task Client_cannot_replace_tenant_or_actor_using_query_parameters()
    {
        using var store = new SettlementStore();
        var foreign = Guid.NewGuid();
        store.Claim().CompanyId = foreign;
        await store.Save();
        using var app = WithStore(store);
        using var client = Client(app, store);
        var result = await client.GetFromJsonAsync<ProjectSettlementSummary>(
            Path(store.ProjectId) + $"?companyId={foreign}&actorId={Guid.NewGuid()}");
        Assert.Equal(0, result!.TotalBlockerCount);
    }

    [Fact]
    public async Task OpenApi_publishes_dtos_and_only_the_read_operation()
    {
        using var client = factory.CreateSecureClient();
        var response = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var path = document.RootElement.GetProperty("paths").GetProperty("/api/v1/projects/{projectId}/settlement");
        Assert.True(path.TryGetProperty("get", out _));
        Assert.False(path.TryGetProperty("post", out _));
        Assert.Contains(nameof(ProjectSettlementSummary), body);
        Assert.Contains(nameof(ProjectSettlementBlocker), body);
        Assert.Contains(nameof(SettlementEvaluationGap), body);
        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", body);
    }

    [Fact]
    public async Task Settlement_has_no_write_or_finalize_route()
    {
        using var store = new SettlementStore();
        await store.Save();
        using var app = WithStore(store);
        using var client = Client(app, store);
        Assert.Equal(HttpStatusCode.MethodNotAllowed,
            (await client.PostAsJsonAsync(Path(store.ProjectId), new { })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.PostAsJsonAsync(Path(store.ProjectId) + "/finalize", new { })).StatusCode);
    }

    private WebApplicationFactory<Program> WithStore(SettlementStore store) => factory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AhdahDbContext>();
            services.AddScoped(_ => store.Open());
        }));

    private static string Path(Guid projectId) => $"/api/v1/projects/{projectId}/settlement";

    private static HttpClient Client(WebApplicationFactory<Program> app, SettlementStore store)
    {
        var client = app.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        var token = app.Services.GetRequiredService<IAccessTokenService>().CreateToken(new(
            store.UserId, store.CompanyId, store.Role, "Settlement Test"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        return client;
    }
}
