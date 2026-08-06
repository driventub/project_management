using Microsoft.Extensions.Configuration;
using ProjectManagement.Infrastructure.Security;
using Xunit;

namespace ProjectManagement.Application.Tests.Security;

public class PasswordHasherTests
{
    private static PasswordHasher BuildHasher()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Security:Pepper"] = "test-pepper" })
            .Build();
        return new PasswordHasher(configuration);
    }

    [Fact]
    public void Verify_ConLaPasswordCorrecta_RetornaTrue()
    {
        var hasher = BuildHasher();
        var (hash, salt) = hasher.Hash("Admin123!");

        Assert.True(hasher.Verify("Admin123!", hash, salt));
    }

    [Fact]
    public void Verify_ConLaPasswordIncorrecta_RetornaFalse()
    {
        var hasher = BuildHasher();
        var (hash, salt) = hasher.Hash("Admin123!");

        Assert.False(hasher.Verify("otra-password", hash, salt));
    }

    [Fact]
    public void Hash_MismaPasswordDosVeces_GeneraSaltsDistintos()
    {
        var hasher = BuildHasher();
        var (hash1, salt1) = hasher.Hash("Admin123!");
        var (hash2, salt2) = hasher.Hash("Admin123!");

        Assert.NotEqual(salt1, salt2);
        Assert.NotEqual(hash1, hash2);
    }
}
