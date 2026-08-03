using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class ProyectoRepository : RepositoryBase<Proyecto, Guid>, IProyectoRepository
{
    public ProyectoRepository(ProjectManagementDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<Proyecto> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? nameFilter,
        CancellationToken cancellationToken = default)
    {
        var query = Set.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(p => EF.Functions.ILike(p.Nombre, $"%{nameFilter}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
