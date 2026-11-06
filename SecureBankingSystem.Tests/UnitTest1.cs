
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
public class AmountValidatorTests
{
    [Theory]
    [InlineData(10, true)]
    [InlineData(0, false)]
    [InlineData(-5, false)]
    public void IsValidAmount_ReturnsExpectedResult(
        decimal amount, bool expected)
    {
        bool result = AmountValidator.IsValidAmount(amount);

        Assert.Equal(expected, result);
    }
}

public class WithdrawalValidatorTests
{
    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(100, 100, true)]
    [InlineData(100, 150, false)]
    [InlineData(100, 0, false)]
    [InlineData(100, -20, false)]
    public void CanWithdraw_ReturnsExpectedResult(
        decimal balance,
        decimal amount,
        bool expected)
    {
        bool result = WithdrawalValidator.CanWithdraw(
        balance, amount);

        Assert.Equal(expected, result);
    }
}

