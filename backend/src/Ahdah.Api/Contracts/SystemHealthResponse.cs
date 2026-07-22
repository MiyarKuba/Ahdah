namespace Ahdah.Api.Contracts;

public sealed record SystemHealthResponse(
    string Service,
    string Status,
    DateTimeOffset TimestampUtc,
    string Environment);
