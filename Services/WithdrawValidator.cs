
namespace secureBankingSystem.Services
{
    public static class WithdrawalValidator
    {
        public static bool CanWithdraw(decimal balance, decimal amount)
        {
            return amount > 0 && amount <= balance;
        }
    }
}
