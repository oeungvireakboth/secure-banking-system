
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

public class BalanceCalculatorTests
{
    [Theory]
    [InlineData(100, 50, 150)]
    [InlineData(100, 1, 101)]
    [InlineData(0, 50, 50)]
    public void TryCalculateDeposit_ReturnsCorrectBalance(
        decimal currentBalance,
        decimal amount,
        decimal expectedBalance)
    {
        bool result = BalanceCalculator.TryCalculateDeposit(
            currentBalance, amount, out decimal newBalance);

        Assert.True(result);
        Assert.Equal(expectedBalance, newBalance);
    }

    [Theory]
    [InlineData(100, 30, 70)]
    [InlineData(100, 100, 0)]
    [InlineData(50, 20, 30)]
    public void TryCalculateWithdrawal_ReturnsCorrectBalance(
        decimal currentBalance,
        decimal amount,
        decimal expectedBalance)
    {
        bool result = BalanceCalculator.TryCalculateWithdrawal(
            currentBalance, amount, out decimal newBalance);

        Assert.True(result);
        Assert.Equal(expectedBalance, newBalance);
    }

    [Fact]
    public void TryCalculateDeposit_RejectsOverflow()
    {
        bool result = BalanceCalculator.TryCalculateDeposit(
            decimal.MaxValue, 1, out decimal newBalance);

        Assert.False(result);
        Assert.Equal(0, newBalance);
    }

    [Theory]
    [InlineData(100, 150)]
    [InlineData(100, 0)]
    [InlineData(100, -10)]
    [InlineData(-50, 10)]
    public void TryCalculateWithdrawal_RejectsInvalidAmounts(
        decimal currentBalance,
        decimal amount)
    {
        bool result = BalanceCalculator.TryCalculateWithdrawal(
            currentBalance, amount, out decimal newBalance);

        Assert.False(result);
        Assert.Equal(0, newBalance);
    }
}


