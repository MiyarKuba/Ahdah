using Ahdah.Infrastructure.Security;

namespace Ahdah.UnitTests;

public sealed class InvitationTokenServiceTests
{
    private readonly InvitationTokenService service = new();

    [Fact]
    public void Generated_tokens_are_non_empty_distinct_and_not_their_hashes()
    {
        var first = service.Generate();
        var second = service.Generate();

        Assert.NotEmpty(first.RawToken);
        Assert.NotEmpty(first.TokenHash);
        Assert.NotEqual(first.RawToken, second.RawToken);
        Assert.NotEqual(first.RawToken, first.TokenHash);
    }

    [Fact]
    public void Hashing_is_deterministic_and_modified_tokens_do_not_match()
    {
        var token = service.Generate();

        Assert.Equal(token.TokenHash, service.Hash(token.RawToken));
        Assert.True(service.Verify(token.RawToken, token.TokenHash));
        Assert.False(service.Verify(token.RawToken + "x", token.TokenHash));
        Assert.NotEqual(token.TokenHash, service.Hash(token.RawToken + "x"));
    }
}
