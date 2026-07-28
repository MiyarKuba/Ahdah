using Ahdah.Infrastructure.Persistence.Generated.Context;
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
