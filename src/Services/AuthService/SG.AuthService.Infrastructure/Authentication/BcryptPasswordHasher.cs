using SG.AuthService.Application.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace SG.AuthService.Infrastructure.Authentication;

public class BcryptPasswordHasher : IPasswordHasher
{
  public string Hash(string password)
  {
    return BC.HashPassword(password);
  }

  public bool Verify(string password, string hash)
  {
    return BC.Verify(password, hash);
  }
}