using Microsoft.EntityFrameworkCore;
using SG.AuthService.Domain.Entities;
using SG.AuthService.Domain.Repositories;

namespace SG.AuthService.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
  private readonly AuthDbContext _context;

  public UserRepository(AuthDbContext context)
  {
    _context = context;
  }

  public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
  }

  public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
  }

  public async Task AddAsync(User user, CancellationToken cancellationToken = default)
  {
    await _context.Users.AddAsync(user,  cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);
  }
}