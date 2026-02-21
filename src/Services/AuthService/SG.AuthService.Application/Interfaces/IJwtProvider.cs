using SG.AuthService.Domain.Entities;

namespace SG.AuthService.Application.Interfaces;

public interface IJwtProvider
{
  string Generate(User user);
}