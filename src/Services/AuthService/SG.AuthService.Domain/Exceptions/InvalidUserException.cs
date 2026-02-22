namespace SG.AuthService.Domain.Exceptions;

public class InvalidUserException : Exception
{
  public InvalidUserException(string message) : base(message)
  {
  }
}