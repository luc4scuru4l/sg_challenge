namespace SG.AuthService.Application.Exceptions;

public class WeakPasswordException : Exception
{
  public WeakPasswordException()
    : base("La contraseña debe tener al menos 6 caracteres y contener al menos un número.")
  {
  }
}