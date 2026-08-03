using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using ProjectManagement.Application.Ports;

namespace ProjectManagement.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private readonly string _pepper;

    public PasswordHasher(IConfiguration configuration)
    {
        _pepper = configuration["Security:Pepper"]
            ?? throw new InvalidOperationException("Security:Pepper configuration value is required.");
    }

    public (string Hash, string Salt) Hash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = Derive(password, saltBytes);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool Verify(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var expectedHash = Convert.FromBase64String(hash);
        var actualHash = Derive(password, saltBytes);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    private byte[] Derive(string password, byte[] saltBytes)
        => Rfc2898DeriveBytes.Pbkdf2(password + _pepper, saltBytes, Iterations, Algorithm, KeySize);
}
