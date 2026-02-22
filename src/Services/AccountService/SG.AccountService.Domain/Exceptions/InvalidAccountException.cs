namespace SG.AccountService.Domain.Exceptions;

public class InvalidAccountException : DomainException
{
  public InvalidAccountException(string message) : base(message)
  {
  }
}