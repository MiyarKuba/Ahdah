using System.Text.Json;

namespace Ahdah.IntegrationTests;

public sealed class ProjectFinancialWriterCoverageTests
{
    [Fact]
    public void Register_covers_W01_through_W48_and_every_current_material_writer_is_gated()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "ProjectFinancialWriterCoverage.json");
        var register = JsonSerializer.Deserialize<WriterCoverageRegister>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(register);
        Assert.Equal("wave2b", register.Version);
        Assert.Equal(Enumerable.Range(1, 48).Select(value => $"W{value:00}"),
            register.Writers.Select(writer => writer.Id).OrderBy(id => id));
        Assert.All(register.Writers.Where(writer => writer.Current && writer.Material), writer =>
        {
            Assert.Equal("GATED", writer.Status);
            Assert.True(writer.T01Gated);
            Assert.True(writer.Revision);
            Assert.NotEmpty(writer.IntegrationCoverage);
        });
        var factNames = typeof(ProjectFinancialWriterCoverageTests).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods())
            .Where(method => method.GetCustomAttributes(typeof(FactAttribute), inherit: true).Length != 0)
            .Select(method => method.Name)
            .ToHashSet(StringComparer.Ordinal);
        Assert.All(register.Writers.SelectMany(writer => writer.IntegrationCoverage),
            testName => Assert.Contains(testName, factNames));
        Assert.DoesNotContain(register.Writers, writer => writer.Status == "NEEDS WAVE 2B");
    }

    private sealed record WriterCoverageRegister(string Version, WriterCoverageEntry[] Writers);

    private sealed record WriterCoverageEntry(
        string Id,
        string Writer,
        bool Current,
        string ProjectLineage,
        bool Material,
        bool? T01Gated,
        bool? Revision,
        string Idempotency,
        bool MultiProject,
        string[] IntegrationCoverage,
        string Status);
}
