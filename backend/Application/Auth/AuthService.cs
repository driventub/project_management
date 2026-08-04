using ProjectManagement.Application.Ports;

namespace ProjectManagement.Application.Auth;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (usuario is null || !_passwordHasher.Verify(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
        {
            return null;
        }

        var (token, expiresAtUtc) = _tokenService.GenerateToken(usuario);
        return new AuthResult(usuario.Id, usuario.Nombre, usuario.Email, token, expiresAtUtc);
    }
}
