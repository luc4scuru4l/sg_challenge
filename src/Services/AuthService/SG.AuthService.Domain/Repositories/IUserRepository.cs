using SG.AuthService.Domain.Entities;

namespace SG.AuthService.Domain.Repositories;

public interface IUserRepository
{
  // Retorna un usuario buscandolo por su Id.
  Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  
  // Retorna un usuario buscandolo por su UserName.
  Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
  
  //Guarda un usuario en la bd
  Task AddAsync(User user, CancellationToken cancellationToken = default);
}