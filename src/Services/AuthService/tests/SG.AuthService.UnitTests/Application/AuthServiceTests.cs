using FluentAssertions;
using Moq;
using AuthServiceClass = SG.AuthService.Application.Services.AuthService;
using SG.AuthService.Application.DTOs;
using SG.AuthService.Application.Interfaces;
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
    public async Task RegisterAsync_WhenUserExists_ShouldThrowInvalidOperationException()
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
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("El nombre de usuario ya está en uso.");
            
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
    public async Task LoginAsync_WhenUserDoesNotExist_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest("lucas", "Password123!");

        _userRepositoryMock
            .Setup(x => x.GetByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); // No existe

        // Act
        Func<Task> act = async () => await _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ShouldThrowUnauthorizedAccessException()
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
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }
}