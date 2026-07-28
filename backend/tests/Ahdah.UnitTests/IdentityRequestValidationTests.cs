using System.ComponentModel.DataAnnotations;
using Ahdah.Application.Identity.Contracts;

namespace Ahdah.UnitTests;

public sealed class IdentityRequestValidationTests
{
    [Fact]
    public void Registration_rejects_non_e164_phone_and_short_company_code()
    {
        var request = new RegisterCompanyRequest(
            "Test Company",
            "ABC",
            "Test Manager",
            "0912345678",
            null,
            "a sufficiently long password");

        var validationResults = Validate(request);

        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(request.CompanyCode)));
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(request.ManagerPhone)));
    }

    [Fact]
    public void Registration_accepts_safe_surrounding_whitespace_for_normalized_identifiers()
    {
        var request = new RegisterCompanyRequest(
            "  Test Company  ",
            "  TEST01  ",
            "  Test Manager  ",
            "  +218912345678  ",
            null,
            "a sufficiently long password");

        var validationResults = Validate(request);

        Assert.Empty(validationResults);
    }

    private static List<ValidationResult> Validate(object value)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);
        return validationResults;
    }
}
