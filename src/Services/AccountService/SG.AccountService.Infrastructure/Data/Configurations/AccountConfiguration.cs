using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SG.AccountService.Domain.Entities;

namespace SG.AccountService.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
  public void Configure(EntityTypeBuilder<Account> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Balance)
      .HasPrecision(18, 4)
      .IsRequired();

    builder.Property(e => e.RowVersion)
      .IsRowVersion();
  }
}