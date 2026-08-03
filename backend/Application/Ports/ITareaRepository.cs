using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Ports;

public interface ITareaRepository : IRepository<Tarea, Guid>
{
    Task<IReadOnlyList<Tarea>> GetByColumnaIdAsync(Guid columnaId, CancellationToken cancellationToken = default);
}
