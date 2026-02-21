using SG.AuthService.Application.Interfaces;
using SG.AuthService.Application.DTOs;
using SG.AuthService.Domain.Entities;
using SG.AuthService.Domain.Repositories;

namespace SG.AuthService.Application.Services;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;
  private readonly IJwtProvider _jwtProvider;
  private readonly IPasswordHasher _passwordHasher;

  public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider, IPasswordHasher passwordHasher)
  {
    _userRepository = userRepository;
    _jwtProvider = jwtProvider;
    _passwordHasher = passwordHasher;
  }

  public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
  {
    var existingUser = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
    if (existingUser != null)
    {
      throw new InvalidOperationException("El nombre de usuario ya está en uso.");
    }
    
    string passwordHash = _passwordHasher.Hash(request.Password);
    var newUser = new User(request.UserName, passwordHash);

    await _userRepository.AddAsync(newUser, cancellationToken);
  }

  public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
    if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
    {
      throw new UnauthorizedAccessException("Credenciales inválidas.");
    }

    string token = _jwtProvider.Generate(user);

    return new LoginResponse(token);
  }
}