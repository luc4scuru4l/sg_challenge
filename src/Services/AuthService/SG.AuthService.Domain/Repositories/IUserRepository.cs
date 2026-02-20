using SG.AuthService.Domain.Entities;

namespace SG.AuthService.Domain.Repositories;

public interface IUserRepository
{
  // Retorna un usuario buscandolo por su Id.
  Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  // Retorna true si un nombre de usuario ya existe.
  Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);

  //Guarda un usuario en la bd
  Task AddAsync(User user, CancellationToken cancellationToken = default);
}