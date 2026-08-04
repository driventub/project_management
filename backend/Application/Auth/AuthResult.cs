namespace ProjectManagement.Application.Auth;

public record AuthResult(Guid UsuarioId, string Nombre, string Email, string Token, DateTime ExpiresAtUtc);
