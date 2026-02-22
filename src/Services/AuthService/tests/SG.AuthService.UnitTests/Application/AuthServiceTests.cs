using FluentAssertions;
using Moq;
using AuthServiceClass = SG.AuthService.Application.Services.AuthService;
using SG.AuthService.Application.DTOs;
using SG.AuthService.Application.Interfaces;
using SG.AuthService.Application.Exceptions;
using SG.AuthService.Domain.Entities;
using SG.AuthService.Domain.Repositories;

namespace SG.AuthService.UnitTests.Application;

public class AuthServiceTests
{
  private readonly Mock<IUserRepository> _userRepositoryMock;
  private readonly Mock<IJwtProvider> _jwtProviderMock;
  private readonly Mock<IPasswordHasher> _passwordHasherMock;
  private readonly AuthServiceClass _sut; // System Under Test

  public AuthServiceTests()
  {
    // Arrange general
    _userRepositoryMock = new Mock<IUserRepository>();
    _jwtProviderMock = new Mock<IJwtProvider>();
    _passwordHasherMock = new Mock<IPasswordHasher>();

    _sut = new AuthServiceClass(
      _userRepositoryMock.Object,
      _jwtProviderMock.Object,
      _passwordHasherMock.Object);
  }

  [Fact]
  public async Task RegisterAsync_WhenUserDoesNotExist_ShouldCreateUser()
  {
    // Arrange
    var request = new RegisterRequest("lucas", "Password123!");

    _userRepositoryMock
      .Setup(x => x.GetByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
      .ReturnsAsync((User?)null);

    _passwordHasherMock
      .Setup(x => x.Hash(request.Password))
      .Returns("hashed_password");

    // Act
    await _sut.RegisterAsync(request);

    // Assert
    // Verificamos que se haya llamado al repositorio para guardar, pasando la entidad armada
    _userRepositoryMock.Verify(x => x.AddAsync(
        It.Is<User>(u => u.UserName == "lucas" && u.PasswordHash == "hashed_password"),
        It.IsAny<CancellationToken>()),
      Times.Once);
  }

  [Fact]
  public async Task RegisterAsync_WhenUserExists_ShouldThrowUserAlreadyExistsException()
  {
    // Arrange
    var request = new RegisterRequest("lucas", "Password123!");
    var existingUser = new User("lucas", "old_hash");

    _userRepositoryMock
      .Setup(x => x.GetByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
      .ReturnsAsync(existingUser); // Simulamos que el usuario YA existe

    // Act
    Func<Task> act = async () => await _sut.RegisterAsync(request);

    // Assert
    await act.Should().ThrowAsync<UserAlreadyExistsException>()
      .WithMessage($"El nombre de usuario '{request.UserName}' ya está en uso.");

    // Verificamos que NUNCA se haya llamado al método de guardar
    _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnToken()
  {
    // Arrange
    var request = new LoginRequest("lucas", "Password123!");
    var existingUser = new User("lucas", "hashed_password");
    var expectedToken = "jwt_token_generado";

    _userRepositoryMock
      .Setup(x => x.GetByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
      .ReturnsAsync(existingUser);

    _passwordHasherMock
      .Setup(x => x.Verify(request.Password, existingUser.PasswordHash))
      .Returns(true); // Simulamos que la contraseña es correcta

    _jwtProviderMock
      .Setup(x => x.Generate(existingUser))
      .Returns(expectedToken);

    // Act
    var result = await _sut.LoginAsync(request);

    // Assert
    result.Should().NotBeNull();
    result.Token.Should().Be(expectedToken);
  }

  [Fact]
  public async Task LoginAsync_WhenUserDoesNotExist_ShouldThrowInvalidCredentialsException()
  {
    // Arrange
    var request = new LoginRequest("lucas", "Password123!");

    _userRepositoryMock
      .Setup(x => x.GetByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
      .ReturnsAsync((User?)null); // No existe

    // Act
    Func<Task> act = async () => await _sut.LoginAsync(request);

    // Assert
    await act.Should().ThrowAsync<InvalidCredentialsException>()
      .WithMessage("Credenciales inválidas.");
  }

  [Fact]
  public async Task LoginAsync_WhenPasswordIsInvalid_ShouldThrowInvalidCredentialsException()
  {
    // Arrange
    var request = new LoginRequest("lucas", "WrongPassword!");
    var existingUser = new User("lucas", "hashed_password");

    _userRepositoryMock
      .Setup(x => x.GetByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
      .ReturnsAsync(existingUser);

    _passwordHasherMock
      .Setup(x => x.Verify(request.Password, existingUser.PasswordHash))
      .Returns(false); // Falla la verificación del hash

    // Act
    Func<Task> act = async () => await _sut.LoginAsync(request);

    // Assert
    await act.Should().ThrowAsync<InvalidCredentialsException>()
      .WithMessage("Credenciales inválidas.");
  }

  [Theory]
  [InlineData(null)] // Falla si no se envía
  [InlineData("")] // Vacío
  [InlineData("   ")] // Solo espacios (al hacer Trim queda vacío)
  [InlineData("12345")] // Menos del mínimo permitido
  public async Task RegisterAsync_WhenPasswordIsMissingOrTooShort_ShouldThrowInvalidPasswordException(
    string shortPassword)
  {
    // Arrange
    var request = new RegisterRequest("lucas", shortPassword);

    // Act
    Func<Task> act = async () => await _sut.RegisterAsync(request);

    // Assert
    await act.Should().ThrowAsync<InvalidPasswordException>()
      .WithMessage($"La contraseña no puede tener menos de {AuthServiceClass.MIN_PASSWORD_LENGTH} caracteres.");

    // Verificamos que falle rápido sin tocar infraestructura
    _userRepositoryMock.Verify(x => x.GetByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
      Times.Never);
    _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    _passwordHasherMock.Verify(x => x.Hash(It.IsAny<string>()), Times.Never);
  }

  [Fact]
  public async Task RegisterAsync_WhenPasswordIsTooLong_ShouldThrowInvalidPasswordException()
  {
    // Arrange
    // Fabricamos un string que supere el límite exacto de la constante + 1
    var tooLongPassword = new string('a', AuthServiceClass.MAX_PASSWORD_LENGTH + 1);
    var request = new RegisterRequest("lucas", tooLongPassword);

    // Act
    Func<Task> act = async () => await _sut.RegisterAsync(request);

    // Assert
    await act.Should().ThrowAsync<InvalidPasswordException>()
      .WithMessage($"La contraseña no puede superar los {AuthServiceClass.MAX_PASSWORD_LENGTH} caracteres.");

    _userRepositoryMock.Verify(x => x.GetByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public async Task RegisterAsync_WhenPasswordHasNoDigits_ShouldThrowInvalidPasswordException()
  {
    // Arrange
    // Fabricamos un string válido en longitud pero sin números
    var noDigitsPassword = new string('a', AuthServiceClass.MIN_PASSWORD_LENGTH + 2);
    var request = new RegisterRequest("lucas", noDigitsPassword);

    // Act
    Func<Task> act = async () => await _sut.RegisterAsync(request);

    // Assert
    await act.Should().ThrowAsync<InvalidPasswordException>()
      .WithMessage("La contraseña debe tener al menos un digito.");

    _userRepositoryMock.Verify(x => x.GetByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Theory]
  [InlineData(null, "Password123!")] // Faltan usuario (null)
  [InlineData("", "Password123!")] // Falta usuario (vacío)
  [InlineData("   ", "Password123!")] // Falta usuario (espacios)
  [InlineData("lucas", null)] // Falta password (null)
  [InlineData("lucas", "")] // Falta password (vacío)
  [InlineData("lucas", "   ")] // Falta password (espacios)
  [InlineData(null, null)] // Faltan ambos
  public async Task LoginAsync_WhenCredentialsAreMissingOrEmpty_ShouldThrowInvalidCredentialsException(string userName,
    string password)
  {
    // Arrange
    var request = new LoginRequest(userName, password);

    // Act
    Func<Task> act = async () => await _sut.LoginAsync(request);

    // Assert
    // Debe lanzar exactamente la excepción de Aplicación que mapea al 401 Unauthorized
    await act.Should().ThrowAsync<InvalidCredentialsException>();

    // Verificación CRÍTICA del "Fail Fast":
    // Nos aseguramos de que no haya consumido recursos yendo a la base de datos, 
    // ni intentado hashear nulos, ni generado tokens por error.
    _userRepositoryMock.Verify(x => x.GetByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
      Times.Never);
    _passwordHasherMock.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    _jwtProviderMock.Verify(x => x.Generate(It.IsAny<User>()), Times.Never);
  }
}