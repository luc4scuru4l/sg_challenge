namespace SG.AccountService.Application.Exceptions;

public class AccountNotFoundException : Exception
{
  public AccountNotFoundException(Guid accountId)
    : base($"No se encontró la cuenta con el ID especificado: {accountId}")
  {
  }
}