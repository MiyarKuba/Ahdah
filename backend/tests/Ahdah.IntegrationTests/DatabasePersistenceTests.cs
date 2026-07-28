using System.Diagnostics;
using Ahdah.Application.Access.Models;
using Ahdah.Infrastructure.Persistence.Generated.Context;
using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.IntegrationTests;

public sealed class DatabasePersistenceTests
{
    [Fact]
    public void Generated_model_maps_exactly_91_ahdah_tables_without_connecting()
    {
        var options = new DbContextOptionsBuilder<AhdahDbContext>()
            .UseNpgsql("Host=localhost;Database=model_metadata_only;Username=unused;Password=unused")
            .Options;

        using var context = new AhdahDbContext(options);

        var mappedTables = context.Model.GetEntityTypes()
            .Select(entityType => (entityType.GetSchema(), entityType.GetTableName()))
            .Where(mapping => mapping.Item2 is not null)
            .Distinct()
            .ToArray();

        Assert.Equal(91, mappedTables.Length);
        Assert.All(mappedTables, mapping => Assert.Equal("ahdah", mapping.Item1));
    }

    [Fact]
    public void Repository_contains_no_database_initialization_or_migrations()
    {
        var repositoryRoot = FindRepositoryRoot();
        var backendRoot = Path.Combine(repositoryRoot, "backend");
        var sourceFiles = Directory.EnumerateFiles(backendRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .ToArray();
        var source = string.Join(Environment.NewLine, sourceFiles.Select(File.ReadAllText));

        Assert.DoesNotContain("Ensure" + "Created(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Ensure" + "Deleted(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Database." + "Migrate(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Database." + "MigrateAsync(", source, StringComparison.Ordinal);

        var migrationDirectories = Directory.EnumerateDirectories(
            backendRoot,
            "*",
            SearchOption.AllDirectories)
            .Where(path => string.Equals(
                Path.GetFileName(path),
                "Migrations",
                StringComparison.OrdinalIgnoreCase));

        Assert.Empty(migrationDirectories);
    }

    [Fact]
    public void Generated_context_contains_no_embedded_connection_string()
    {
        var contextPath = Path.Combine(
            FindRepositoryRoot(),
            "backend", "src", "Ahdah.Infrastructure", "Persistence", "Generated", "Context",
            "AhdahDbContext.cs");
        var source = File.ReadAllText(contextPath);

        Assert.DoesNotContain("OnConfiguring", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Host=", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Password=", source, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Identity_persistence_never_assigns_plaintext_password_to_hash_field()
    {
        var identityServicePath = Path.Combine(
            FindRepositoryRoot(),
            "backend", "src", "Ahdah.Infrastructure", "Identity", "IdentityService.cs");
        var source = File.ReadAllText(identityServicePath);

        Assert.DoesNotContain("PasswordHash = request.Password", source, StringComparison.Ordinal);
        Assert.DoesNotContain("PasswordHash = request.Password.Trim", source, StringComparison.Ordinal);
        Assert.Contains("PasswordHash = passwordHash", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Access_contracts_and_controllers_do_not_expose_persistence_entities_or_hashes()
    {
        var responseTypes = new[]
        {
            typeof(InvitationSummary),
            typeof(CreateInvitationResult),
            typeof(InvitationAcceptanceResult),
            typeof(JoinRequestSubmissionResult),
            typeof(JoinRequestSummary),
            typeof(JoinRequestDecisionResult)
        };

        Assert.All(responseTypes, type => Assert.DoesNotContain(
            type.GetProperties(),
            property => property.Name.Contains("Hash", StringComparison.OrdinalIgnoreCase)
                || property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase)));

        var controllerDirectory = Path.Combine(
            FindRepositoryRoot(), "backend", "src", "Ahdah.Api", "Controllers");
        var accessControllerSource = string.Join(
            Environment.NewLine,
            new[] { "InvitationsController.cs", "JoinRequestsController.cs" }
                .Select(name => File.ReadAllText(Path.Combine(controllerDirectory, name))));

        Assert.DoesNotContain("Ahdah.Infrastructure.Persistence.Generated", accessControllerSource);
        Assert.DoesNotContain("InvitationCodeHash", accessControllerSource);
        Assert.DoesNotContain("PasswordHash", accessControllerSource);
    }

    [Fact]
    public void Access_persistence_enforces_tenant_scope_and_never_persists_plaintext_secrets()
    {
        var infrastructureRoot = Path.Combine(
            FindRepositoryRoot(), "backend", "src", "Ahdah.Infrastructure", "Access");
        var invitationSource = File.ReadAllText(Path.Combine(
            infrastructureRoot, "Invitations", "InvitationService.cs"));
        var joinRequestSource = File.ReadAllText(Path.Combine(
            infrastructureRoot, "JoinRequests", "JoinRequestService.cs"));

        Assert.Contains("invitation.CompanyId == tenant.Value.CompanyId", invitationSource);
        Assert.Contains("company_id = {tenant.Value.CompanyId}", invitationSource);
        Assert.Contains("request.CompanyId == tenant.Value.CompanyId", joinRequestSource);
        Assert.Contains("company_id = {companyId} AND join_request_id", joinRequestSource);
        Assert.Contains("company_id = {companyId} AND user_id = {userId}", joinRequestSource);
        Assert.DoesNotContain("PasswordHash = request.Password", invitationSource);
        Assert.DoesNotContain("PasswordHash = request.Password", joinRequestSource);
        Assert.Contains("passwordHashingService.HashPassword(request.Password)", invitationSource);
        Assert.Contains("var passwordHash = passwordHashingService.HashPassword(request.Password)", joinRequestSource);
        Assert.Contains("PasswordHash = passwordHash", joinRequestSource);
        Assert.Contains("BeginTransactionAsync", joinRequestSource);
        Assert.Contains("dbContext.AppUsers.Add(applicant)", joinRequestSource);
        Assert.Contains("dbContext.JoinRequests.Add(joinRequest)", joinRequestSource);
        Assert.DoesNotContain("FindAsync", invitationSource);
        Assert.DoesNotContain("FindAsync", joinRequestSource);

        var approvalSource = Between(
            joinRequestSource,
            "public async Task<AccessResult<JoinRequestDecisionResult>> ApproveAsync",
            "public async Task<AccessResult<JoinRequestDecisionResult>> RejectAsync");
        Assert.Contains("LockTenantApplicantAsync", approvalSource);
        Assert.Contains("applicant.Role = assignedRole", approvalSource);
        Assert.DoesNotContain("new AppUser", approvalSource);

        var rejectionSource = Between(
            joinRequestSource,
            "public async Task<AccessResult<JoinRequestDecisionResult>> RejectAsync",
            "private Task<JoinRequest?> LockTenantRequestAsync");
        Assert.Contains("applicant.Status = AccessRoleRules.UserStatusForJoinRejection()", rejectionSource);
        Assert.DoesNotContain("Remove(", rejectionSource);
        Assert.DoesNotContain("Delete", rejectionSource);
    }

    [Fact]
    public void Generated_invitation_model_has_only_a_hash_field_for_the_invitation_code()
    {
        var propertyNames = typeof(Invitation).GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.Contains(nameof(Invitation.InvitationCodeHash), propertyNames);
        Assert.DoesNotContain("InvitationCode", propertyNames);
        Assert.DoesNotContain("RawToken", propertyNames);
        Assert.DoesNotContain("Token", propertyNames);
    }

    [Fact]
    public void Generated_persistence_files_are_unchanged_in_git()
    {
        var repositoryRoot = FindRepositoryRoot();
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "status --porcelain -- backend/src/Ahdah.Infrastructure/Persistence/Generated",
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }) ?? throw new InvalidOperationException("Could not start Git safety check.");

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        Assert.Equal(0, process.ExitCode);
        Assert.True(string.IsNullOrWhiteSpace(output));
    }

    private static string Between(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(start >= 0 && end > start);
        return source[start..end];
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AGENTS.md")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate the repository root.");
    }
}
