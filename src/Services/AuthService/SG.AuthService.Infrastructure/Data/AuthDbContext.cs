using Microsoft.EntityFrameworkCore;
using SG.AuthService.Domain.Entities;
using System.Reflection;

namespace SG.AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext
{
  public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}