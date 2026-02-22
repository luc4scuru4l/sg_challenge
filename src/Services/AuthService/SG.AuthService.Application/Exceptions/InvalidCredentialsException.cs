namespace SG.AuthService.Application.Exceptions;

public class InvalidCredentialsException : Exception
{
  public InvalidCredentialsException()
    : base("Credenciales inválidas.")
  {
  }
}