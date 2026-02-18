namespace SG.AccountService.Domain.Exceptions;

public class InsufficientFundsException : DomainException
{
  public InsufficientFundsException() : base("Fondos insuficientes para realizar la operación.")
  {
    
  }
}