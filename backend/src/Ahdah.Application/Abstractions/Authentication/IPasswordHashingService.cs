namespace Ahdah.Application.Abstractions.Authentication;

public interface IPasswordHashingService
{
    string HashPassword(string password);

    PasswordVerificationResult VerifyPassword(string passwordHash, string providedPassword);
}

public enum PasswordVerificationResult
{
    Failed,
    Success,
    SuccessRehashNeeded
}
