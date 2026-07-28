using System.Security.Cryptography;
using System.Text;
using Ahdah.Application.Abstractions.Security;

namespace Ahdah.Infrastructure.Security;

public sealed class InvitationTokenService : IInvitationTokenService
{
    private const int TokenByteLength = 32;

    public InvitationToken Generate()
    {
        var rawToken = Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenByteLength));
        return new InvitationToken(rawToken, Hash(rawToken));
    }

    public string Hash(string rawToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);
        return Base64UrlEncode(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
    }

    public bool Verify(string rawToken, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        var computedHash = Encoding.ASCII.GetBytes(Hash(rawToken));
        var expectedHash = Encoding.ASCII.GetBytes(storedHash);
        return computedHash.Length == expectedHash.Length
            && CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
    }

    private static string Base64UrlEncode(byte[] value) =>
        Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
