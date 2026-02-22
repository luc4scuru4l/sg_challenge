using SG.AuthService.Application.Interfaces;
using SG.AuthService.Application.DTOs;
using SG.AuthService.Application.Exceptions;
using SG.AuthService.Domain.Entities;
using SG.AuthService.Domain.Repositories;

namespace SG.AuthService.Application.Services;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;
  private readonly IJwtProvider _jwtProvider;
  private readonly IPasswordHasher _passwordHasher;
  public const int MAX_PASSWORD_LENGTH = 32;
  public const int MIN_PASSWORD_LENGTH = 8;

  public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider, IPasswordHasher passwordHasher)
  {
    _userRepository = userRepository;
    _jwtProvider = jwtProvider;
    _passwordHasher = passwordHasher;
  }

  public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
  {
    var cleanPassword = request.Password?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(cleanPassword) || cleanPassword.Length < MIN_PASSWORD_LENGTH)
      throw new InvalidPasswordException($"La contraseña no puede tener menos de {MIN_PASSWORD_LENGTH} caracteres.");
    if (cleanPassword.Length > MAX_PASSWORD_LENGTH)
      throw new InvalidPasswordException($"La contraseña no puede superar los {MAX_PASSWORD_LENGTH} caracteres.");
    if (!cleanPassword.Any(char.IsDigit))
      throw new InvalidPasswordException("La contraseña debe tener al menos un digito.");
    
    var existingUser = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
    if (existingUser != null)
      throw new UserAlreadyExistsException(request.UserName);
    
    string passwordHash = _passwordHasher.Hash(cleanPassword);
    var newUser = new User(request.UserName, passwordHash);

    await _userRepository.AddAsync(newUser, cancellationToken);
  }

  public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
      throw new InvalidCredentialsException();
    
    var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
    if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
    {
      throw new InvalidCredentialsException();
    }

    string token = _jwtProvider.Generate(user);

    return new LoginResponse(token);
  }
}