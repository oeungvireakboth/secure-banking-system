
using Microsoft.Data.Sqlite;
using secureBankingSystem.Data;
using secureBankingSystem.Services;
using Xunit;

namespace SecureBankingSystem.Tests
{
    public class BankTests
    {
        [Fact]
        public void Deposit_DatabaseFailure_KeepsOriginalBalance()
        {
            string dbPath = Path.Combine(
                Path.GetTempPath(),
                $"bank_test_{Guid.NewGuid()}.db");

            TextReader originalInput = Console.In;
            TextWriter originalOutput = Console.Out;

            try
            {
                var database = new DatabaseService(
                    $"Data Source={dbPath};Pooling=False");

                database.InitializeDatabase();

                database.SaveUser(
                    "TestUser",
                    "test@example.com",
                    25,
                    "1234567890",
                    "test-hash",
                    100m);

                // Create a trigger to simulate a database failure
                using (var connection = new SqliteConnection(
                    $"Data Source={dbPath};Pooling=False"))
                {
                    connection.Open();

                    using var command = connection.CreateCommand();

                    command.CommandText = @"
                        CREATE TRIGGER FailTransactionInsert
                        BEFORE INSERT ON Transactions
                        BEGIN
                            SELECT RAISE(ABORT, 'Simulated database failure');
                        END;
                    ";

                    command.ExecuteNonQuery();
                }

                var bank = new Bank(database);
                var user = database.LoadUsers()[0];

                // Simulate entering $50
                Console.SetIn(new StringReader("50"));
                var output = new StringWriter();
                Console.SetOut(output);

                bank.Deposit(user);

                // Balance should remain unchanged
                Assert.Equal(100m, user.Balance);

                // No transaction should be recorded
                Assert.Empty(user.Transactions);

                // Confirm the database error message appeared
                Assert.Contains(
                    "Deposit failed due to a database error.",
                    output.ToString());
            }
            finally
            {
                Console.SetIn(originalInput);
                Console.SetOut(originalOutput);

                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public void Withdraw_DatabaseFailure_KeepsOriginalBalance()
        {
            string dbPath = Path.Combine(
                Path.GetTempPath(),
                $"bank_withdraw_test_{Guid.NewGuid()}.db");

            TextReader originalInput = Console.In;
            TextWriter originalOutput = Console.Out;

            try
            {
                var database = new DatabaseService(
                    $"Data Source={dbPath};Pooling=False");

                database.InitializeDatabase();

                database.SaveUser(
                    "TestUser",
                    "test@example.com",
                    25,
                    "1234567890",
                    "test-hash",
                    100m);

                // Create a trigger to simulate a database failure
                using (var connection = new SqliteConnection(
                    $"Data Source={dbPath};Pooling=False"))
                {
                    connection.Open();

                    using var command = connection.CreateCommand();

                    command.CommandText = @"
                CREATE TRIGGER FailTransactionInsert
                BEFORE INSERT ON Transactions
                BEGIN
                    SELECT RAISE(ABORT, 'Simulated database failure');
                END;
            ";

                    command.ExecuteNonQuery();
                }

                var bank = new Bank(database);
                var user = database.LoadUsers()[0];

                // Simulate withdrawing $50
                Console.SetIn(new StringReader("50"));
                var output = new StringWriter();
                Console.SetOut(output);

                bank.Withdraw(user);

                // Balance should remain unchanged
                Assert.Equal(100m, user.Balance);

                // No transaction should be recorded
                Assert.Empty(user.Transactions);

                // Confirm the database error message appeared
                Assert.Contains(
                    "Withdrawal failed due to a database error.",
                    output.ToString());
            }
            finally
            {
                Console.SetIn(originalInput);
                Console.SetOut(originalOutput);

                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

    }
}
