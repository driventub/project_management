using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Seed;

/// <summary>
/// Seed users for local/dev login. Both use password "Admin123!" and were hashed
/// with PBKDF2-SHA256 (100k iterations) using the pepper in .env.example
/// (SECURITY__PEPPER). Regenerate if that pepper value ever changes.
/// </summary>
public static class UsuarioSeed
{
    public static readonly Guid Admin1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Admin2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static IReadOnlyList<Usuario> GetSeedUsers()
        => new List<Usuario>
        {
            new()
            {
                Id = Admin1Id,
                Nombre = "Admin Uno",
                Email = "admin1@projectmanagement.local",
                PasswordHash = "FD4xp8ol72qeLIg+2QE4Cuf+rZMgcD4ScntYE3ZXC2c=",
                PasswordSalt = "6fuwTuaiZzsMmq9otOFh+A=="
            },
            new()
            {
                Id = Admin2Id,
                Nombre = "Admin Dos",
                Email = "admin2@projectmanagement.local",
                PasswordHash = "exgmd5l/KuNfnyKKVWG70RxQ0Kf63/pV65ibZICnj8I=",
                PasswordSalt = "8FZRfC/qHVAlLO0GEHQl8A=="
            }
        };
}
