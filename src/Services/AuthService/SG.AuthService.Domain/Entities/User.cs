namespace SG.AuthService.Domain.Entities;

public class User
{
  public Guid Id { get; private set; }
  public string UserName { get; private set; } = null!;
  public string PasswordHash { get; private set; } = null!;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }
  
  protected User()
  {
  }

  public User(string userName, string passwordHash)
  {
    if (string.IsNullOrWhiteSpace(userName))
      throw new ArgumentException("El userName es requerido.");

    if (string.IsNullOrWhiteSpace(passwordHash))
      throw new ArgumentException("El hash de la contraseña es requerido.");

    Id = Guid.NewGuid();
    UserName = userName.Trim();
    PasswordHash = passwordHash;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }
  
}