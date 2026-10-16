
using Xunit;
using secureBankingSystem.Services;

namespace SecureBankingSystem.Tests;

public class PasswordServiceTests
{
        
    [Fact]
    public void VerifyPassword_RejectsInvalidBase64()
    {
        var service = new PasswordService();

        Assert.False(
            service.VerifyPassword("Password123", "!!!:!!!")
        );
    }

    [Fact]
    public void VerifyPassword_RejectsMalformedHash()
    {
        var service = new PasswordService();

        Assert.False(
            service.VerifyPassword("Password123", "not-a-valid-hash")
        );
    }

    [Fact]
    public void HashPassword_DoesNotStorePlainTextPassword()
    {
        var service = new PasswordService();
        string password = "Password123";

        string hash = service.HashPassword(password);

        Assert.NotEqual(password, hash);
        Assert.DoesNotContain(password, hash);
    }

    [Fact]
    public void VerifyPassword_AcceptsCorrectPassword()
    {
        var service = new PasswordService();
        string password = "Password123";
        string hash = service.HashPassword(password);

        Assert.True(service.VerifyPassword(password, hash));
    }

    [Fact]
    public void VerifyPassword_RejectsIncorrectPassword()
    {
        var service = new PasswordService();
        string hash = service.HashPassword("Password123");

        Assert.False(service.VerifyPassword("WrongPassword", hash));
    }
}
