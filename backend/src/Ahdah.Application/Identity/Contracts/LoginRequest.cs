using System.ComponentModel.DataAnnotations;

namespace Ahdah.Application.Identity.Contracts;

public sealed record LoginRequest
{
    public LoginRequest(string phoneNumber, string password)
    {
        PhoneNumber = phoneNumber;
        Password = password;
    }

    [Required]
    [StringLength(40)]
    [RegularExpression("^\\s*\\+[1-9][0-9]{7,14}\\s*$")]
    public string PhoneNumber { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 1)]
    public string Password { get; init; }
}
