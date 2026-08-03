using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class ColumnaRepository : RepositoryBase<Columna, Guid>, IColumnaRepository
{
    public ColumnaRepository(ProjectManagementDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Columna>> GetByProyectoIdAsync(Guid proyectoId, CancellationToken cancellationToken = default)
        => await Set
            .Where(c => c.ProyectoId == proyectoId)
            .OrderBy(c => c.Orden)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasTareasAsync(Guid columnaId, CancellationToken cancellationToken = default)
        => await Context.Tareas.AnyAsync(t => t.ColumnaId == columnaId, cancellationToken);
}
