using Ahdah.Application.Abstractions.Authentication;
using Ahdah.Infrastructure.Authentication;

namespace Ahdah.UnitTests;

public sealed class PasswordHashingServiceTests
{
    private readonly PasswordHashingService _service = new();

    [Fact]
    public void Generated_hash_differs_from_plaintext_password()
    {
        const string password = "correct horse battery staple";

        var hash = _service.HashPassword(password);

        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void Valid_password_verifies_successfully()
    {
        const string password = "correct horse battery staple";
        var hash = _service.HashPassword(password);

        var result = _service.VerifyPassword(hash, password);

        Assert.Contains(result, new[]
        {
            PasswordVerificationResult.Success,
            PasswordVerificationResult.SuccessRehashNeeded
        });
    }

    [Fact]
    public void Invalid_password_fails_verification()
    {
        var hash = _service.HashPassword("correct horse battery staple");

        var result = _service.VerifyPassword(hash, "this password is incorrect");

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public void Repeated_hashes_of_same_password_are_not_assumed_identical()
    {
        const string password = "correct horse battery staple";

        var firstHash = _service.HashPassword(password);
        var secondHash = _service.HashPassword(password);

        Assert.NotEqual(firstHash, secondHash);
        Assert.Equal(PasswordVerificationResult.Success, _service.VerifyPassword(firstHash, password));
        Assert.Equal(PasswordVerificationResult.Success, _service.VerifyPassword(secondHash, password));
    }
}
