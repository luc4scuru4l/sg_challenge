namespace SG.AccountService.Domain.Exceptions;

public abstract class DomainException : Exception
{
  protected DomainException(string message) : base(message)
  {
  }
}