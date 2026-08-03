using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class UsuarioRepository : RepositoryBase<Usuario, Guid>, IUsuarioRepository
{
    public UsuarioRepository(ProjectManagementDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await Set.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}
