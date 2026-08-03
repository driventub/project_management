using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Ports;

public interface IColumnaRepository : IRepository<Columna, Guid>
{
    Task<IReadOnlyList<Columna>> GetByProyectoIdAsync(Guid proyectoId, CancellationToken cancellationToken = default);
    Task<bool> HasTareasAsync(Guid columnaId, CancellationToken cancellationToken = default);
}
