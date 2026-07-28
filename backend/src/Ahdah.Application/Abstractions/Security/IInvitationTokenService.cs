namespace Ahdah.Application.Abstractions.Security;

public sealed record InvitationToken(string RawToken, string TokenHash);

public interface IInvitationTokenService
{
    InvitationToken Generate();

    string Hash(string rawToken);

    bool Verify(string rawToken, string storedHash);
}
