
namespace secureBankingSystem.Services
{
    public static class BalanceCalculator
    {
        public static bool TryCalculateDeposit(
            decimal currentBalance,
            decimal amount,
            out decimal newBalance)
        {
            newBalance = 0;

            if (currentBalance < 0 || amount <= 0)
                return false;

            try
            {
                newBalance = checked(currentBalance + amount);
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        public static bool TryCalculateWithdrawal(
            decimal currentBalance,
            decimal amount,
            out decimal newBalance)
        {
            newBalance = 0;

            if (currentBalance < 0 || amount <= 0 || amount > currentBalance)
                return false;

            newBalance = currentBalance - amount;
            return true;
        }
    }
}
