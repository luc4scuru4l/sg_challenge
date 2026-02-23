namespace SG.AccountService.Application.Exceptions;

public class ConcurrencyConflictException : Exception
{
  public ConcurrencyConflictException()
    : base("El estado de la cuenta cambió desde que se inició la operación. Por favor, reintente.")
  {
  }
}