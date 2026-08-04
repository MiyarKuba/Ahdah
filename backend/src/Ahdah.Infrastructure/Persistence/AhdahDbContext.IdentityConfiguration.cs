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

        modelBuilder.Entity<Project>()
            .Property(project => project.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<ProjectOwner>()
            .Property(owner => owner.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<Advance>()
            .Property(advance => advance.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<FundingSource>()
            .Property(source => source.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<MoneyTransfer>()
            .Property(transfer => transfer.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<UserAdvanceBalance>()
            .Property(balance => balance.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<Expense>()
            .Property(expense => expense.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<ExpenseCategory>()
            .Property(category => category.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<ExpenseDocument>()
            .Property(document => document.VersionNumber)
            .IsConcurrencyToken();

        modelBuilder.Entity<PersonalClaim>()
            .Property(claim => claim.VersionNumber)
            .IsConcurrencyToken();
    }
}
