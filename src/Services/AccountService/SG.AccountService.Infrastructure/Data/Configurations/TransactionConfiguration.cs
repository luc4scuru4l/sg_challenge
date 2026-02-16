using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SG.AccountService.Domain.Entities;

namespace SG.AccountService.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(e => e.Balance)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}