namespace SG.AuthService.Application.Exceptions;

public class UserAlreadyExistsException : Exception
{
  public UserAlreadyExistsException(string userName)
    : base($"El nombre de usuario '{userName}' ya está en uso.")
  {
  }
}