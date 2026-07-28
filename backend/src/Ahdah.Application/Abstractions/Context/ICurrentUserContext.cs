namespace Ahdah.Application.Abstractions.Context;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    Guid? CompanyId { get; }

    string? Role { get; }
}
