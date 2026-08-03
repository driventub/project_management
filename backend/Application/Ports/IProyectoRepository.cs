using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Ports;

public interface IProyectoRepository : IRepository<Proyecto, Guid>
{
    Task<(IReadOnlyList<Proyecto> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? nameFilter,
        CancellationToken cancellationToken = default);
}
