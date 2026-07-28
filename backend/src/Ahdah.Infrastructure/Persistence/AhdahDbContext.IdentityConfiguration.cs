using Ahdah.Infrastructure.Persistence.Generated.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ahdah.Infrastructure.Persistence.Generated.Context;

public partial class AhdahDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .Property(user => user.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<Company>()
            .Property(company => company.VersionNumber)
            .IsConcurrencyToken();
    }
}
