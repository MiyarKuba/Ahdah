namespace Ahdah.UnitTests;

public sealed class FoundationSmokeTests
{
    [Fact]
    public void Test_project_targets_dotnet_10()
    {
        var targetFramework = typeof(FoundationSmokeTests).Assembly
            .GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), inherit: false)
            .Cast<System.Runtime.Versioning.TargetFrameworkAttribute>()
            .Single();

        Assert.Equal(".NETCoreApp,Version=v10.0", targetFramework.FrameworkName);
    }
}
