namespace SG.AccountService.Domain.Exceptions;

public class InvalidMoneyException : DomainException
{
  public InvalidMoneyException(string message)
    : base(message)
  {
  }
}