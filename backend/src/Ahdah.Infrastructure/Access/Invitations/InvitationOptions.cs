using System.ComponentModel.DataAnnotations;

namespace Ahdah.Infrastructure.Access.Invitations;

public sealed class InvitationOptions
{
    public const string SectionName = "Access:Invitations";

    [Range(1, 720)]
    public int LifetimeHours { get; init; }
}

public static class InvitationExpiryCalculator
{
    public static DateTime Calculate(TimeProvider timeProvider, InvitationOptions options) =>
        timeProvider.GetUtcNow().AddHours(options.LifetimeHours).UtcDateTime;
}
