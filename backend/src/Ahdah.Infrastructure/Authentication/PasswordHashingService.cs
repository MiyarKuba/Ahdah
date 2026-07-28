using Ahdah.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Ahdah.Infrastructure.Authentication;

public sealed class PasswordHashingService : IPasswordHashingService
{
    private readonly PasswordHasher<PasswordHashingUser> _passwordHasher = new();
    private readonly PasswordHashingUser _user = new();

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return _passwordHasher.HashPassword(_user, password);
    }

    public Ahdah.Application.Abstractions.Authentication.PasswordVerificationResult VerifyPassword(
        string passwordHash,
        string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        return _passwordHasher.VerifyHashedPassword(_user, passwordHash, providedPassword) switch
        {
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success =>
                Ahdah.Application.Abstractions.Authentication.PasswordVerificationResult.Success,
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded =>
                Ahdah.Application.Abstractions.Authentication.PasswordVerificationResult.SuccessRehashNeeded,
            _ => Ahdah.Application.Abstractions.Authentication.PasswordVerificationResult.Failed
        };
    }

    private sealed class PasswordHashingUser;
}
