using Moq;
using ProjectManagement.Application.Auth;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;
using Xunit;

namespace ProjectManagement.Application.Tests.Auth;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_EmailDesconocido_RetornaNullSinVerificarPassword()
    {
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.GetByEmailAsync("nadie@x.com", It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();
        var service = new AuthService(usuarioRepository.Object, passwordHasher.Object, tokenService.Object);

        var resultado = await service.LoginAsync(new LoginRequest("nadie@x.com", "cualquier"));

        Assert.Null(resultado);
        passwordHasher.Verify(p => p.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_PasswordInvalida_RetornaNullSinGenerarToken()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nombre = "Admin", Email = "admin@x.com", PasswordHash = "hash", PasswordSalt = "salt" };

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(p => p.Verify("mala", usuario.PasswordHash, usuario.PasswordSalt)).Returns(false);

        var tokenService = new Mock<ITokenService>();
        var service = new AuthService(usuarioRepository.Object, passwordHasher.Object, tokenService.Object);

        var resultado = await service.LoginAsync(new LoginRequest(usuario.Email, "mala"));

        Assert.Null(resultado);
        tokenService.Verify(t => t.GenerateToken(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CredencialesValidas_RetornaAuthResultConToken()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nombre = "Admin", Email = "admin@x.com", PasswordHash = "hash", PasswordSalt = "salt" };
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(p => p.Verify("Admin123!", usuario.PasswordHash, usuario.PasswordSalt)).Returns(true);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.GenerateToken(usuario)).Returns(("token-generado", expiresAt));

        var service = new AuthService(usuarioRepository.Object, passwordHasher.Object, tokenService.Object);

        var resultado = await service.LoginAsync(new LoginRequest(usuario.Email, "Admin123!"));

        Assert.NotNull(resultado);
        Assert.Equal(usuario.Id, resultado!.UsuarioId);
        Assert.Equal(usuario.Email, resultado.Email);
        Assert.Equal("token-generado", resultado.Token);
        Assert.Equal(expiresAt, resultado.ExpiresAtUtc);
    }
}
