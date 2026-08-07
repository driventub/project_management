using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class TareaRepository : RepositoryBase<Tarea, Guid>, ITareaRepository
{
    public TareaRepository(ProjectManagementDbContext context) : base(context)
    {
    }

    public override async Task<Tarea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Set
            .Include(t => t.Columna)
            .Include(t => t.Responsable)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Tarea>> GetByColumnaIdAsync(Guid columnaId, CancellationToken cancellationToken = default)
        => await Set
            .Include(t => t.Columna)
            .Include(t => t.Responsable)
            .Where(t => t.ColumnaId == columnaId)
            .OrderBy(t => t.Orden)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Tarea>> GetByProyectoIdAsync(Guid proyectoId, CancellationToken cancellationToken = default)
        => await Set
            .Include(t => t.Columna)
            .Include(t => t.Responsable)
            .Where(t => t.Columna.ProyectoId == proyectoId)
            .OrderBy(t => t.ColumnaId)
            .ThenBy(t => t.Orden)
            .ToListAsync(cancellationToken);
}
