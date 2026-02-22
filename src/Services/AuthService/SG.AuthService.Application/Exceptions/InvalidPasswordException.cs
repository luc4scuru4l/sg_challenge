namespace SG.AuthService.Application.Exceptions;

public class InvalidPasswordException : Exception
{
  public InvalidPasswordException(string message) : base(message)
  {
  }
}