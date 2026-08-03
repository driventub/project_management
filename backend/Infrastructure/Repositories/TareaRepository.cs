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

    public async Task<IReadOnlyList<Tarea>> GetByColumnaIdAsync(Guid columnaId, CancellationToken cancellationToken = default)
        => await Set
            .Where(t => t.ColumnaId == columnaId)
            .OrderBy(t => t.Orden)
            .ToListAsync(cancellationToken);
}
