using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SG.AuthService.Domain.Entities;

namespace SG.AuthService.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    // 1. Nombre explícito de la tabla (por convención en plural)
    builder.ToTable("Users");

    // 2. Clave Primaria
    builder.HasKey(u => u.Id);

    // 3. Configuración de Propiedades
    builder.Property(u => u.UserName)
      .IsRequired()
      .HasMaxLength(50); // Le ponemos un límite sensato para no gastar espacio de más

    builder.Property(u => u.PasswordHash)
      .IsRequired()
      .HasMaxLength(255); // Espacio de sobra para algoritmos como BCrypt o Argon2

    builder.Property(u => u.IsActive)
      .IsRequired()
      .HasDefaultValue(true); // Aunque tu constructor ya lo hace, está bueno que la BD también lo sepa

    builder.Property(u => u.CreatedAt)
      .IsRequired();

    // 4. ÍNDICES (¡La magia del rendimiento!)
    // Como vamos a buscar todo el tiempo por UserName para el Login y el Registro, 
    // necesitamos un índice único. Esto evita que SQL Server escanee toda la tabla fila por fila.
    builder.HasIndex(u => u.UserName)
      .IsUnique();
  }
}