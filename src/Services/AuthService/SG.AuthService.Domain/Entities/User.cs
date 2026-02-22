using SG.AuthService.Domain.Exceptions;
namespace SG.AuthService.Domain.Entities;

public class User
{
  public Guid Id { get; private set; }
  public string UserName { get; private set; } = null!;
  public string PasswordHash { get; private set; } = null!;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public const int MAX_USERNAME_LENGTH = 50;
  
  protected User()
  {
  }

  public User(string userName, string passwordHash)
  {
    var cleanUserName = userName?.Trim();
    if (string.IsNullOrWhiteSpace(cleanUserName))
      throw new InvalidUserException("El userName es requerido.");
    
    if (cleanUserName.Length > MAX_USERNAME_LENGTH)
      throw new InvalidUserException($"El nombre de usuario no puede superar los {MAX_USERNAME_LENGTH} caracteres.");
    
    if (string.IsNullOrWhiteSpace(passwordHash))
      throw new InvalidUserException("El hash de la contraseña es requerido.");

    Id = Guid.NewGuid();
    UserName = cleanUserName;
    PasswordHash = passwordHash;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }
}