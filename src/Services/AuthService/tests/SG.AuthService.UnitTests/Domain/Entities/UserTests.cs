using FluentAssertions;
using SG.AuthService.Domain.Entities;
using SG.AuthService.Domain.Exceptions;

namespace SG.AuthService.UnitTests.Domain.Entities;

public class UserTests
{
  [Fact]
  public void Constructor_WithValidData_ShouldInitializeUser()
  {
    // Arrange
    // Le mandamos espacios al principio y al final para verificar que el Trim() funcione
    var validUserName = "   admin_jperez   ";
    var validPasswordHash = "hashed_password_123_xyz";

    // Act
    var user = new User(validUserName, validPasswordHash);

    // Assert
    user.Id.Should().NotBeEmpty();
    user.UserName.Should().Be("admin_jperez"); // Verificamos que se haya limpiado
    user.PasswordHash.Should().Be(validPasswordHash);
    user.IsActive.Should().BeTrue(); // Regla de negocio: nace activo
    user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData(null)]
  public void Constructor_WithInvalidUserName_ShouldThrowInvalidUserException(string invalidUserName)
  {
    // Arrange
    var validPasswordHash = "hashed_password_123_xyz";

    // Act
    Action act = () => new User(invalidUserName, validPasswordHash);

    // Assert
    act.Should().Throw<InvalidUserException>()
      .WithMessage("El userName es requerido.");
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData(null)]
  public void Constructor_WithInvalidPasswordHash_ShouldThrowInvalidUserException(string invalidPasswordHash)
  {
    // Arrange
    var validUserName = "admin_jperez";

    // Act
    Action act = () => new User(validUserName, invalidPasswordHash);

    // Assert
    act.Should().Throw<InvalidUserException>()
      .WithMessage("El hash de la contraseña es requerido.");
  }

  [Fact]
  public void Constructor_WhenUserNameIsTooLong_ShouldThrowInvalidUserException()
  {
    // Arrange
    // Crea un string de 51 letras 'a' (MaxUserNameLength + 1)
    var tooLongUserName = new string('a', User.MAX_USERNAME_LENGTH + 1);
    var tooShortUserName = new string('a', User.MIN_USERNAME_LENGTH - 1);
    var validPasswordHash = "hashed_password_123";

    // Act
    Action actMax = () => new User(tooLongUserName, validPasswordHash);
    Action actMin = () => new User(tooShortUserName, validPasswordHash);

    // Assert
    actMax.Should().Throw<InvalidUserException>()
      .WithMessage($"El nombre de usuario no puede superar los {User.MAX_USERNAME_LENGTH} caracteres.");
    actMin.Should().Throw<InvalidUserException>()
      .WithMessage($"El nombre de usuario no puede tener menos de {User.MIN_USERNAME_LENGTH} caracteres.");
  }
}