namespace Ahdah.IntegrationTests;

public sealed class ApiProjectSmokeTests
{
    [Fact]
    public void Api_project_reference_is_available()
    {
        Assert.Equal("Ahdah.Api", typeof(Program).Assembly.GetName().Name);
    }
}
