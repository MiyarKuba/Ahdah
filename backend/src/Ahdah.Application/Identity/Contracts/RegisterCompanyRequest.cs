using System.ComponentModel.DataAnnotations;

namespace Ahdah.Application.Identity.Contracts;

public sealed record RegisterCompanyRequest
{
    public RegisterCompanyRequest(
        string companyName,
        string companyCode,
        string managerFullName,
        string managerPhone,
        string? managerEmail,
        string password)
    {
        CompanyName = companyName;
        CompanyCode = companyCode;
        ManagerFullName = managerFullName;
        ManagerPhone = managerPhone;
        ManagerEmail = managerEmail;
        Password = password;
    }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string CompanyName { get; init; }

    [Required]
    [StringLength(40, MinimumLength = 6)]
    [RegularExpression("^\\s*[A-Za-z0-9]{6,20}\\s*$")]
    public string CompanyCode { get; init; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression("^(?=.*\\S)[^\\r\\n]+$")]
    public string ManagerFullName { get; init; }

    [Required]
    [StringLength(40)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public string ManagerPhone { get; init; }

    [EmailAddress]
    [StringLength(254)]
    public string? ManagerEmail { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; init; }
}
