using SG.AccountService.Domain.Entities;

namespace SG.AccountService.Application.Interfaces;

public interface IAccountRepository
{
  //Obtiene una cuenta a través de su ID
  Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  // Añade una nueva cuenta
  Task AddAsync(Account account, CancellationToken cancellationToken = default);

  // Guarda el nuevo saldo de la cuenta y el log de la transacción
  Task UpdateWithTransactionAsync(Account account, Transaction transaction,
    CancellationToken cancellationToken = default);
}