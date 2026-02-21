using SG.AuthService.Application.DTOs;

namespace SG.AuthService.Application.Interfaces;

// El contrato del servicio
public interface IAuthService
{
  // Registra un usuario.
  Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

  // Retorna un token vigente a un usuario.
  Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}