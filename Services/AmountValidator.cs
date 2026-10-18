namespace secureBankingSystem.Services
{
    public static class AmountValidator
    {
        // Returns true if the amount is valid (greater than zero); otherwise returns false
        public static bool IsValidAmount(decimal amount)
        {
            return amount > 0;
        }
    }
}