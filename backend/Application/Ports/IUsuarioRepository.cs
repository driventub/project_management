using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Ports;

public interface IUsuarioRepository : IRepository<Usuario, Guid>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
