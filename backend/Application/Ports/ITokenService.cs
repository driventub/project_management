using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Ports;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(Usuario usuario);
}
