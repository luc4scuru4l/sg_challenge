namespace SG.AccountService.Application.Exceptions;

public class UnauthorizedException : Exception
{
  public UnauthorizedException() : base("Token inválido o vencido")
  {
  }
}