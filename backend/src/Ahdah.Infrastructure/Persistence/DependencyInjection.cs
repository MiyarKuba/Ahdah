using Ahdah.Infrastructure.Persistence.Generated.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahdah.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddAhdahPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AhdahDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AhdahDatabase' is not configured.");
        }

        services.AddDbContext<AhdahDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.SetPostgresVersion(18, 0)));

        return services;
    }
}
